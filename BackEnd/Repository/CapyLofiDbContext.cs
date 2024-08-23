using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Gọi base để đảm bảo cấu hình các bảng liên quan đến Identity
        base.OnModelCreating(modelBuilder);

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

        // Cấu hình các thuộc tính cơ bản của thực thể
        ConfigureBaseEntityProperties(modelBuilder);
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



