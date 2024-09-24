using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Commons;
using Repository.Interfaces;
using Repository.Repositories;
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
            services.AddIdentity<User, IdentityRole<int>>() // IdentityRole<int> to use int as primary key
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

            // Add generic repository
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

            return services;
        }
    }


}