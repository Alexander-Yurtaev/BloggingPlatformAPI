using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BloggingPlatformAPI.DataContext;

public static class BloggingPlatformDataContextExtensions
{
    public static IServiceCollection AddBloggingPlatformDataContext(this IServiceCollection services,
        string? connectionString = null)
    {
        if (connectionString is null)
        {
            NpgsqlConnectionStringBuilder builder = new();

            builder.Host = "localhost"; // или IP-адрес
            builder.Port = 5432; // порт по умолчанию для PostgreSQL
            builder.Database = "postgres";
            builder.Username = Environment.GetEnvironmentVariable("POSTGRES_USER");
            builder.Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            builder.CommandTimeout = 30; // таймаут выполнения команд

            connectionString = builder.ConnectionString;
        }

        services.AddDbContext<BloggingPlatformDataContext>(options =>
            {
                options.UseNpgsql(connectionString);
            },
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient);

        return services;
    }
}