using System.Reflection;
using Application.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureServices
{

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(typeof(MappingProfiles));

        return services;
    }
}