using AppApi.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AppApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("AppConnectionString"));
        });

        builder.Services.AddMapster();

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation($"Iniciando execução da migration");

            var context = services.GetRequiredService<AppDbContext>();

            context.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
