using System.Text;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Identity;
using Infrastructure.AuthService;
using Infrastructure.Database;
using Infrastructure.Database.Interceptors;
using Infrastructure.DateTimeService;
using Infrastructure.Options;
using Infrastructure.Repository;
using Infrastructure.TwelveDataApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddMemoryCache();

        services.AddScoped<AuditableEntitySaveChanges>();
        
        services.AddSingleton<IStockClient, StockClient>();
        services.AddSingleton<IMatchingEngine, MatchingEngine>();
        
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IFeeProvider, FeeProvider>();
        
        services.AddTransient<TwelveDataHeaderMiddleware>();

        services.ConfigureOptions<TwelveDataApiOptionSetup>();
        services.ConfigureOptions<JwtSettingsOptionsSetup>();
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

        // Auth
        services.AddScoped<IIdentityService, IdentityService>();

        var jwtSettings = configuration.GetSection(nameof(JwtSettingsOptions));

        var tokenValidationParamters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Secret"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true,
        };
        services.AddSingleton(tokenValidationParamters);


        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.TokenValidationParameters = tokenValidationParamters;

        });
        services.AddAuthorization();

        return services;
    }
}