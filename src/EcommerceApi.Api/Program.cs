using EcommerceApi.Api.Middleware;
using EcommerceApi.Application.Extensions;
using EcommerceApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

// BOOTSTRAP LOGGER
// Temporary logger to capture errors during app initialization
// If builder.Build() or app.Run() blow up, this records the error
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Replaces ASP.NET Core's default logger with Serilog
    builder.Services.AddSerilog((services, lc) =>
    {
        lc.ReadFrom.Configuration(builder.Configuration)  // reads from appsettings.json
          .ReadFrom.Services(services)                    // allows injecting services into enrichers
          .Enrich.FromLogContext()                        // enables LogContext.PushProperty()
          .Enrich.WithProperty("Application", "EcommerceApi")
          .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);

        if (builder.Environment.IsProduction())
            lc.WriteTo.Console(new CompactJsonFormatter());
        else
            lc.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");
    });

    // Add services to the container.
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Development", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.Use(async (context, next) =>
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString("N")[..8];

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            await next(context);
        }
    });

    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (ctx, _, ex) =>
                 ex != null || ctx.Response.StatusCode >= 500 ? LogEventLevel.Error :
                 ctx.Response.StatusCode >= 400 ? LogEventLevel.Warning :
                 LogEventLevel.Information;

        options.EnrichDiagnosticContext = (diag, ctx) =>
        {
            diag.Set("ClientIP", ctx.Connection.RemoteIpAddress?.ToString());
            diag.Set("UserAgent", ctx.Request.Headers["UserAgent"].ToString());
        };
    });

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    app.UseHttpsRedirection();
    app.UseMiddleware<TenantResolutionMiddleware>();
    app.UseCors("Development");
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // Catches catastrophical failures during startup
    Log.Fatal(ex, "Application terminated unexpectedly during setup");
}
finally
{
    // Ensures all buffered logs are flushed before the process exits
    Log.CloseAndFlush();
}