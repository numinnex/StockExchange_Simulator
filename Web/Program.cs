using Application;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);


var presentationAssembly = typeof(Presentation.AssemblyReference).Assembly;

//used for debugging purposes
//builder.Services.AddScoped<AuthFilter>();

builder.Services.AddControllers()
    .AddApplicationPart(presentationAssembly);
builder.Services.AddSignalR();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo() { Title = "Stock Exchange Api", Version = "v1" });
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var adminRole = new IdentityRole("Admin"); 
        await roleManager.CreateAsync(adminRole);
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        var userRole = new IdentityRole("User"); 
        await roleManager.CreateAsync(userRole);
    }

    var val = await dbContext.Fees.FindAsync(1);
    if (val is null)
    {
        await dbContext.Fees.AddAsync(new Fee
        {
            Id = 1,
            MakerFee = 0.1m,
            TakerFee = 0.5m
        });
        await dbContext.SaveChangesAsync();
    }
}
    
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .WithMethods("GET", "POST", "PUT", "DELETE")
        .AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PriceHub>("/price");
app.MapControllers();


app.Run();
