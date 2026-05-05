using Microsoft.Extensions.DependencyInjection;

namespace EcommerceApi.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
