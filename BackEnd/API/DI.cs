using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Repository;
using Repository.Commons;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Interfaces;
using Service.Mappers;
using Service.Services;
using System.Security.Claims;
using System.Text;
using JwtSettings = Repository.Commons.JwtSettings;

namespace API
{
    public static class DI
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddDbContext<CapyLofiDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            // Configure Identity with role management
            services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<CapyLofiDbContext>()
                .AddDefaultTokenProviders();

            // Register IPasswordHasher<User>
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Register EmailService for Identity
            services.AddScoped<IEmailSender, EmailService>();

            // Add common services
            services.AddScoped<ICurrentTime, CurrentTime>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddHttpContextAccessor();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IAuthenService, AuthenService>();

            // Register AutoMapper
            services.AddAutoMapper(typeof(MapperConfigProfile).Assembly);

            // Register EmailService
            services.AddScoped<EmailService>();

            // Add UNIT OF WORK
            services.AddProjectUnitOfWork();

            // Add SignalR
            services.AddSignalR();

            // Add CORS configuration
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policyBuilder =>
                {
                    policyBuilder.WithOrigins("http://localhost:5278", "https://localhost:5278", "http://localhost:3000", "https://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // Add Identity Core services
            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<CapyLofiDbContext>()
                .AddApiEndpoints();

            // Add Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
                };

                // Handle challenge event
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new ApiResult<string>
                        {
                            Success = false,
                            Data = null,
                            Message = "User is not authenticated. Please log in."
                        });
                        return context.Response.WriteAsync(result);
                    }
                };
            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserPolicy", policy =>
                    policy.RequireClaim(ClaimTypes.Role, "User"));
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireClaim(ClaimTypes.Role, "Admin"));
            });

            // Add Swagger configuration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CapyLofi API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter your JWT token below without the 'Bearer' prefix.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
            });

            return services;
        }

        public static IServiceCollection AddProjectUnitOfWork(this IServiceCollection services)
        {
            // Add repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBackgroundRepository, BackgroundRepository>();
            services.AddScoped<IMusicRepository, MusicRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
            services.AddScoped<IChatInvitationRepository, ChatInvitationRepository>();

            // Add generic repositories
            services.AddScoped<IGenericRepository<Background>, GenericRepository<Background>>();
            services.AddScoped<IGenericRepository<Music>, GenericRepository<Music>>();
            services.AddScoped<IGenericRepository<Message>, GenericRepository<Message>>();
            services.AddScoped<IGenericRepository<ChatRoom>, GenericRepository<ChatRoom>>();
            services.AddScoped<IGenericRepository<ChatInvitation>, GenericRepository<ChatInvitation>>();

            // Add services
            services.AddScoped<IBackgroundItemService, BackgroundItemService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMusicService, MusicService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IChatRoomService, ChatRoomService>();
            services.AddScoped<IChatInvitationService, ChatInvitationService>();

            // Add unit of work
            services.AddScoped<IUnitOfWork, UnitOFWork>();
            services.AddControllers();

            return services;
        }
    }



}