using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Commons;
using Repository.Interfaces;
using Repository.Repositories;
using Service;
using Service.Interfaces;
using Service.Mappers;
using Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JwtSettings = Repository.Commons.JwtSettings;

namespace API
{
    public static class DI
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind JwtSettings
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            // Add ApplicationDbContext with SQL Server
            services.AddDbContext<CapyLofiDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PhucString")));

            services.AddScoped<ICurrentTime, CurrentTime>();
            services.AddScoped<IClaimsService, ClaimsService>();

            // Add UNIT OF WORK
            services.AddProjectUnitOfWork();

            // Register IAuthenticationService
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Others
            services.AddAutoMapper(typeof(MapperConfigProfile).Assembly);
            services.AddHttpContextAccessor();

            return services;
        }

        public static IServiceCollection AddProjectUnitOfWork(this IServiceCollection services)
        {
            // Add repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IBackgroundRepository, BackgroundRepository>();
            services.AddScoped<IMusicRepository, MusicRepository>();

            // Add generic repository
            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IGenericRepository<Background>, GenericRepository<Background>>();
            services.AddScoped<IGenericRepository<Music>, GenericRepository<Music>>();

            // Add services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBackgroundItemService, BackgroundItemService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMusicService, MusicService>();
            services.AddSingleton<TokenGenerators>();

            // Add unit of work
            services.AddScoped<IUnitOfWork, UnitOFWork>();

            return services;
        }
    }
}
