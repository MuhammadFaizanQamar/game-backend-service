using System.Text;
using System.Text.Json.Serialization;
using GameBackend.Application.UseCases.Auth;
using GameBackend.Application.UseCases.Players;
using GameBackend.Core.Interfaces;
using GameBackend.Infrastructure.Persistence;
using GameBackend.Infrastructure.Repositories;
using GameBackend.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameBackend.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<GameDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IPlayerRepository, PlayerRepository>();

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));

        // Use Cases
        services.AddScoped<RegisterPlayerUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<GetPlayerProfileUseCase>();
        services.AddScoped<UpdatePlayerProfileUseCase>();

        // JWT Authentication
        var jwtKey = Configuration["Jwt:Key"]!;
        services
            .AddAuthentication(static auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("JWT Token validated successfully");
                        return Task.CompletedTask;
                    }
                };
            });

        // CORS — allows any game client to connect
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy
                    .AllowAnyOrigin()
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .AllowAnyHeader()));

        // Controllers with JSON options (inspired by DragonPals)
        services
            .AddControllers()
            .AddJsonOptions(static options =>
            {
                // Enums as strings — readable API responses
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                // Case insensitive — forgiving for clients
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                // Don't send null fields — cleaner responses
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            // Health check endpoint — same pattern as DragonPals
            endpoints.Map("/", static context =>
                context.Response.WriteAsync("GameBackend API is running"));
        });
    }
}