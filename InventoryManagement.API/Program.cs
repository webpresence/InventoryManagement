using AspNetCoreRateLimit;
using InventoryManagement.API.Mapping;
using InventoryManagement.API.Security;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Identity;
using InventoryManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Initializes a new instance for creating a builder for the application.
var builder = WebApplication.CreateBuilder(args);

// Adds services to the DI container and configures the DbContext with SQL Server using the connection string from configuration.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Configures AutoMapper for mapping domain models to DTOs.
builder.Services.AddAutoMapper(typeof(InventoryItemProfile).Assembly);

// Configures authentication using JWT bearer tokens.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configures CORS to allow specific origins.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedSites", builder =>
    {
        builder.WithOrigins("https://example.com", "https://www.contoso.com")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Configures rate limiting based on client IP.
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Adds domain services to the DI container.
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Adds support for controllers.
builder.Services.AddControllers();

// Configures Swagger/OpenAPI for API documentation in development environments.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Builds the application.
var app = builder.Build();

// Applies any pending database migrations at application startup to ensure the database schema is up-to-date.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.Migrate(); // Apply pending migrations.
    }
}

// Configures the HTTP request pipeline for development environment.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowedSites"); // Enables CORS with the specified policy.

app.UseMiddleware<AntiXssMiddleware>(); // Adds middleware to sanitize inputs against XSS.

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS.

app.UseAuthentication(); // Enables authentication middleware.
app.UseAuthorization(); // Enables authorization middleware.

app.UseIpRateLimiting(); // Enables IP rate limiting middleware.

app.MapControllers(); // Maps controller actions to routes.

// Runs the application.
app.Run();

// Defines a partial Program class to support an entry point for the application.
public partial class Program { }
