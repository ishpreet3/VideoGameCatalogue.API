using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using VideoGameCatalogue.API.Data;

namespace VideoGameCatalogue.API.Tests.Api
{
    /// <summary>
    /// Hosts the real API in memory, swapping SQL Server for a private in-memory SQLite database
    /// (containing the seed data) and the system clock for a controllable fake one.
    /// </summary>
    public sealed class VideoGameCatalogueApiFactory : WebApplicationFactory<Program>
    {
        public static readonly DateTimeOffset StartTime = new(2025, 6, 1, 12, 0, 0, TimeSpan.Zero);

        private readonly SqliteTestDatabase _database = new(includeSeedData: true);

        public FakeTimeProvider TimeProvider { get; } = new(StartTime);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<VideoGameContext>();
                services.RemoveAll<DbContextOptions<VideoGameContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<VideoGameContext>>();
                services.AddScoped(_ => _database.CreateContext());

                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(TimeProvider);
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _database.Dispose();
        }
    }
}
