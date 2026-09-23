using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.API.Data;
using VideoGameCatalogue.API.Repositories;

public partial class Program
{
    private const string CorsPolicy = "Frontend";

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services
        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();

        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Database configuration
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        builder.Services.AddDbContext<VideoGameContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddScoped<IVideoGameRepository, VideoGameRepository>();

        // Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Integration tests run in the "Testing" environment and create their own database.
        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VideoGameContext>();
            dbContext.Database.Migrate();
        }

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(CorsPolicy);
        app.MapControllers();

        app.Run();
    }
}
