using Application.Common.Intefaces;
using Application.Common.Interfaces.Repository;
using Domain.Identity;
using Infrastructure.Database;
using Infrastructure.Database.Interceptors;
using Infrastructure.Options;
using Infrastructure.Repository;
using Infrastructure.TwelveDataApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddMemoryCache();
        
        services.AddScoped<AuditableEntitySaveChanges>();
        services.AddSingleton<IStockClient, StockClient>();

        services.AddScoped<IStockRepository, StockRepository>();
        
        services.AddTransient<TwelveDataHeaderMiddleware>();
        
        
        services.ConfigureOptions<TwelveDataApiOptionSetup>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });


        services.AddHttpClient<IStockClient, StockClient>((sp, client) =>
        {
            var twelveDataOptions = sp.GetService<IOptions<TwelveDataApiOptions>>()!.Value;
            client.BaseAddress = new Uri(twelveDataOptions.Uri);

            //client.DefaultRequestHeaders.Add("X-RapidAPI-Key", twelveDataOptions.Key);
            //client.DefaultRequestHeaders.Add("X-RapidAPI-Host", twelveDataOptions.Host);

        }).AddHttpMessageHandler<TwelveDataHeaderMiddleware>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();


        services.AddAuthentication();
        services.AddAuthorization();

        return services;
    }
}