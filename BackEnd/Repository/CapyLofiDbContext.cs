﻿using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;



public class CapyLofiDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public CapyLofiDbContext(DbContextOptions<CapyLofiDbContext> options) : base(options)
    {
    }

    public DbSet<LearningSession> LearningSessions { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Music> Musics { get; set; }
    public DbSet<Background> Backgrounds { get; set; }
    public DbSet<UserMusic> UserMusics { get; set; }
    public DbSet<UserBackground> UserBackgrounds { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }

    //Chatroom
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<UserChatRoom> UserChatRooms { get; set; }
    public DbSet<ChatInvitation> ChatInvitations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        // Cấu hình tùy chỉnh cho User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.DisplayName).HasMaxLength(255);
            entity.Property(e => e.PhotoUrl).HasMaxLength(255);
        });

        // Cấu hình các thực thể khác như LearningSessions, Orders, Musics, Backgrounds, UserMusic, UserBackgrounds, Feedbacks
        modelBuilder.Entity<LearningSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.LearningSessions)
                .HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OrderDate).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<Music>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.MusicUrl).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<Background>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.BackgroundUrl).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<UserMusic>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MusicId });
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserMusics)
                .HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Music)
                .WithMany(m => m.UserMusics)
                .HasForeignKey(e => e.MusicId);
        });

        modelBuilder.Entity<UserBackground>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BackgroundId });
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserBackgrounds)
                .HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Background)
                .WithMany(b => b.UserBackgrounds)
                .HasForeignKey(e => e.BackgroundId);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeedbackText).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            entity.HasOne(e => e.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(e => e.UserId);
        });

        // Fluent API configuration for UserChatRoom composite key
        modelBuilder.Entity<UserChatRoom>()
            .HasKey(uc => new { uc.UserId, uc.ChatRoomId });

        modelBuilder.Entity<UserChatRoom>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserChatRooms)
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserChatRoom>()
            .HasOne(uc => uc.ChatRoom)
            .WithMany(cr => cr.UserChatRooms)
            .HasForeignKey(uc => uc.ChatRoomId);

        // Configure relationships for Message entity
        modelBuilder.Entity<Message>()
            .HasOne(m => m.ChatRoom)
            .WithMany(cr => cr.Messages)
            .HasForeignKey(m => m.ChatRoomId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.UserId);

        // Configure relationships for ChatInvitation entity
        modelBuilder.Entity<ChatInvitation>()
            .HasOne(ci => ci.ChatRoom)
            .WithMany(cr => cr.ChatInvitations)
            .HasForeignKey(ci => ci.ChatRoomId);

        modelBuilder.Entity<ChatInvitation>()
            .HasOne(ci => ci.User)
            .WithMany()
            .HasForeignKey(ci => ci.UserId);

        // Cấu hình các thuộc tính cơ bản của thực thể
        ConfigureBaseEntityProperties(modelBuilder);
        // Gọi base để đảm bảo cấu hình các bảng liên quan đến Identity
        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureBaseEntityProperties(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).Property<DateTime>("CreatedAt");
                modelBuilder.Entity(entityType.ClrType).Property<int?>("CreatedBy");
                modelBuilder.Entity(entityType.ClrType).Property<DateTime?>("ModifiedAt");
                modelBuilder.Entity(entityType.ClrType).Property<int?>("ModifiedBy");
                modelBuilder.Entity(entityType.ClrType).Property<DateTime?>("DeletedAt");
                modelBuilder.Entity(entityType.ClrType).Property<int?>("DeletedBy");
                modelBuilder.Entity(entityType.ClrType).Property<bool?>("IsDeleted").HasDefaultValue(false);
            }
        }
    }
}



