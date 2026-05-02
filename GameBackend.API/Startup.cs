using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using GameBackend.Application.UseCases.Auth;
using GameBackend.Application.UseCases.Leaderboards;
using GameBackend.Application.UseCases.Players;
using GameBackend.Application.UseCases.Sessions;
using GameBackend.Core.Interfaces;
using GameBackend.Infrastructure.Cache;
using GameBackend.Infrastructure.Persistence;
using GameBackend.Infrastructure.Repositories;
using GameBackend.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using GameBackend.API.Hubs;
using GameBackend.Application.Validators.Auth;
using GameBackend.API.Middleware;
using GameBackend.API.RateLimiting;
using GameBackend.Application.UseCases.Admin;
using Microsoft.OpenApi.Models;

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

        // Redis via Upstash REST API
        var redisSettings = new RedisSettings
        {
            Url = Configuration["Redis:Url"]!,
            Token = Configuration["Redis:Token"]!
        };
        services.AddSingleton<ICacheService>(_ => new RedisCacheService(redisSettings));

        // Sessions
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<StartSessionUseCase>();
        services.AddScoped<EndSessionUseCase>();
        services.AddScoped<GetSessionHistoryUseCase>();
        services.AddScoped<GetSessionStatsUseCase>();

        // Use Cases
        services.AddScoped<RegisterPlayerUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<GetPlayerProfileUseCase>();
        services.AddScoped<UpdatePlayerProfileUseCase>();

        // Leaderboard
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<SubmitScoreUseCase>();
        services.AddScoped<GetTopLeaderboardUseCase>();
        services.AddScoped<GetPlayerRankUseCase>();

        //Refresh Token
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUseCase>();

        // Admin
        services.AddScoped<GetAllPlayersUseCase>();
        services.AddScoped<BanPlayerUseCase>();
        services.AddScoped<UnbanPlayerUseCase>();
        services.AddScoped<UpdatePlayerRoleUseCase>();
        services.AddScoped<DeletePlayerUseCase>();

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
                    .WithOrigins("http://localhost:5152", "null", "file://")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

        // Controllers with JSON options
        services
            .AddControllers()
            .AddJsonOptions(static options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
            });

        // FluentValidation — modern API
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Game Backend API",
                Version = "v1",
                Description = "A generic game backend API supporting authentication, leaderboards, sessions and player profiles. Built with ASP.NET Core 8, PostgreSQL, Redis and Clean Architecture.",
                Contact = new OpenApiContact
                {
                    Name = "Muhammad Faizan Qamar",
                    Url = new Uri("https://github.com/MuhammadFaizanQamar/game-backend-service")
                }
            });

            // Add JWT auth to Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token like this: Bearer {your token}"
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

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });
        services.AddGameBackendRateLimiting();

        // SignalR
        services.AddSignalR();
        services.AddScoped<ILeaderboardNotificationService, LeaderboardNotificationService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();

        //if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors();
        app.UseRouting();
        app.UseCors();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<LeaderboardHub>("/hubs/leaderboard");
            endpoints.Map("/", static context =>
                context.Response.WriteAsync("GameBackend API is running"));
        });
    }
}