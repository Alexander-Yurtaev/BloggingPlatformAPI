using BloggingPlatformAPI.Shared.Models;
using Microsoft.EntityFrameworkCore;
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
}