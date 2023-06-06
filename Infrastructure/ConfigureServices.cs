using System.Text;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Identity;
using Infrastructure.AuthService;
using Infrastructure.BackgroundWorkers;
using Infrastructure.Database;
using Infrastructure.Database.Interceptors;
using Infrastructure.DateTimeService;
using Infrastructure.Options;
using Infrastructure.Repository;
using Infrastructure.StockService;
using Infrastructure.TradeService;
using Infrastructure.TwelveDataApi;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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
        services.AddSingleton<IMatchingEngine, MatchingEngine.MatchingEngine>();

        services.AddHostedService<AccountValueSnapshotWorker>();
        
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IFeeRepository, FeeRepository>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ITradeListener, TradeListener>();
        services.AddScoped<IFeeProvider, FeeProvider>();
        
        services.AddTransient<TwelveDataHeaderMiddleware>();

        services.AddScoped<IStockUtils, StockUtils>();
        
        services.AddScoped<IUriService>(provider =>
        {
            var accessor = provider.GetRequiredService<IHttpContextAccessor>();
            var request = accessor.HttpContext!.Request;
            var absoluteUri = $"{request.Scheme}://{request.Host.ToUriComponent()}{request.Path}";

            return new UriService(absoluteUri);
        });

        services.ConfigureOptions<TwelveDataApiOptionSetup>();
        services.ConfigureOptions<JwtSettingsOptionsSetup>();
        services.ConfigureOptions<MatchingEngineOptionsSetup>();
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
        var jwtSettings = new JwtSettingsOptions();
        configuration.Bind(nameof(JwtSettingsOptions) ,jwtSettings );

        services.AddScoped<IIdentityService, IdentityService>();

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.TokenValidationParameters = tokenValidationParameters;
        });
        
        services.AddSingleton(tokenValidationParameters);
        return services;
    }
}