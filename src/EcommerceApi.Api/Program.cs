using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    Console.WriteLine($"[REQUEST] {context.Request.Method} {path}");

    await next(context);
    Console.WriteLine($"[RESPONSE] Status: {context.Response.StatusCode}");
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseCors("Development");

app.UseAuthorization();

app.MapControllers();

app.Run();
