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
            NpgsqlConnectionStringBuilder builder = new()
            {
                Host = "localhost", // или IP-адрес
                Port = 5432, // порт по умолчанию для PostgreSQL
                Database = "blog_platform",
                Username = Environment.GetEnvironmentVariable("POSTGRES_USER"),
                Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"),
                CommandTimeout = 30 // таймаут выполнения команд
            };

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

        // Обновляем UpdatedAt для измененных записей или DeletedAt для удаленных записей
        foreach (EntityEntry<Post> entityEntry in ChangeTracker.Entries<Post>().Where(p => p.State == EntityState.Modified))
        {
            if (entityEntry.Entity is { IsDeleted: true, DeletedAt: null })
            {
                entityEntry.Entity.DeletedAt = now;
            }
            else
            {
                entityEntry.Entity.UpdatedAt = now;
            }
        }
    }
}