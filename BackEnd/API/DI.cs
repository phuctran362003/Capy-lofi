using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Commons;
using Repository.Interfaces;
using Repository.Repositories;
using Service;
using Service.Interfaces;
using Service.Mappers;
using Service.Services;
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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add Identity services
        services.AddIdentity<User, IdentityRole<int>>() // IdentityRole<int> để sử dụng khóa chính là int
            .AddEntityFrameworkStores<CapyLofiDbContext>()
            .AddDefaultTokenProviders();
        
        
        // Đăng ký EmailService cho Identity
        services.AddScoped<IEmailSender<User>, EmailService>();


        // Add common services
        services.AddScoped<ICurrentTime, CurrentTime>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddHttpContextAccessor();

        // Register IAuthenticationService
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IOtpService, OtpService>(); // Đăng ký OtpService

        // Register AutoMapper
        services.AddAutoMapper(typeof(MapperConfigProfile).Assembly);

        // Register EmailService
        services.AddScoped<EmailService>(); // Thêm dòng này để đăng ký EmailService

        // Add UNIT OF WORK
        services.AddProjectUnitOfWork();

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
        services.AddScoped<IGenericRepository<Background>, GenericRepository<Background>>();
        services.AddScoped<IGenericRepository<Music>, GenericRepository<Music>>();

        // Add services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IBackgroundItemService, BackgroundItemService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IMusicService, MusicService>();
        services.AddScoped<IOtpService, OtpService>(); 

        // Add Token Generators
        services.AddSingleton<TokenGenerators>();

        // Add unit of work
        services.AddScoped<IUnitOfWork, UnitOFWork>();

        return services;
    }
}

}
