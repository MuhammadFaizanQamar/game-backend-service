using System.Text;
using System.Text.Json.Serialization;
using GameBackend.Application.UseCases.Auth;
using GameBackend.Core.Interfaces;
using GameBackend.Infrastructure.Persistence;
using GameBackend.Infrastructure.Repositories;
using GameBackend.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Auth.Service;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<GameDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));

        // Use Cases
        services.AddScoped<RegisterPlayerUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUseCase>();

        // JWT Authentication
        var jwtKey = Configuration["Jwt:Key"]!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.UseSecurityTokenValidators = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auth Service API",
                Version = "v1",
                Description = "Handles player authentication — register, login, refresh token, logout"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.Map("/", static context =>
                context.Response.WriteAsync("Auth Service is running"));
        });
    }
}