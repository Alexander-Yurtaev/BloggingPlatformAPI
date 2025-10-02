using BloggingPlatformAPI.DataContext;
using BloggingPlatformAPI.MinimalApi.MapGets;
using BloggingPlatformAPI.Repositories;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace BloggingPlatformAPI.MinimalApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Добавляем Serilog в хост
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddAuthorization();

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                    var filePath = Path.Combine(System.AppContext.BaseDirectory, "MyApi.xml");
                    c.IncludeXmlComments(filePath);
                });

                builder.Services.AddBloggingPlatformDataContext();
                builder.Services.AddScoped<IRepository, Repository>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.AddBlogMapGets(app.Logger);

                app.Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
