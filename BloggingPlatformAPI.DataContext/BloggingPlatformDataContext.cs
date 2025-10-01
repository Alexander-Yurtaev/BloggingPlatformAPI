using BloggingPlatformAPI.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;

namespace BloggingPlatformAPI.DataContext;

public class BloggingPlatformDataContext : DbContext
{
    public BloggingPlatformDataContext()
    {
        
    }

    public BloggingPlatformDataContext(DbContextOptions<BloggingPlatformDataContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            NpgsqlConnectionStringBuilder builder = new();

            builder.Host = "localhost"; // или IP-адрес
            builder.Port = 5432; // порт по умолчанию для PostgreSQL
            builder.Database = "blog_platform";
            builder.Username = Environment.GetEnvironmentVariable("POSTGRES_USER");
            builder.Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            builder.CommandTimeout = 30; // таймаут выполнения команд

            optionsBuilder.UseNpgsql(builder.ConnectionString);
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var now = DateTimeOffset.UtcNow;

        // Обновляем CreatedAt для новых записей
        foreach (EntityEntry<Post> entityEntry in ChangeTracker.Entries<Post>().Where(p => p.State == EntityState.Added))
        {
            entityEntry.Entity.CreatedAt = now;
            entityEntry.Entity.UpdatedAt = now;
        }

        // Обновляем UpdatedAt для измененных записей
        foreach (EntityEntry<Post> entityEntry in ChangeTracker.Entries<Post>().Where(p => p.State == EntityState.Modified))
        {
            entityEntry.Entity.UpdatedAt = now;
        }
    }
}