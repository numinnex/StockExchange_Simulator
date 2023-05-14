using Application;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


var presentationAssembly = typeof(Presentation.AssemblyReference).Assembly;
builder.Services.AddControllers()
    .AddApplicationPart(presentationAssembly);

builder.Services.AddApplicationServices();


builder.Services.AddInfrastructureServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1" , new OpenApiInfo() {Title = "Stock API" , Version = "v1"});
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement()
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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

app.Run();
