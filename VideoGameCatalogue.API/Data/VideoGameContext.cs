using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoGameCatalogue.API.Models;

namespace VideoGameCatalogue.API.Data
{
    public class VideoGameContext : DbContext
    {
        private static readonly ValueConverter<DateTime, DateTime> UtcConverter = new(
            toDatabase => toDatabase,
            fromDatabase => DateTime.SpecifyKind(fromDatabase, DateTimeKind.Utc));

        public VideoGameContext(DbContextOptions<VideoGameContext> options) : base(options)
        {
        }

        public DbSet<VideoGame> VideoGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure VideoGame entity
            modelBuilder.Entity<VideoGame>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Developer)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Publisher)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Genre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Platform)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Price)
                    .HasPrecision(10, 2);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                // Timestamps are always stored in UTC, but datetime2 has no time zone, so values read
                // back would otherwise be DateTimeKind.Unspecified and serialised without a trailing "Z".
                entity.Property(e => e.CreatedAt)
                    .HasConversion(UtcConverter);

                entity.Property(e => e.UpdatedAt)
                    .HasConversion(UtcConverter);
            });

            // Seed initial data
            modelBuilder.Entity<VideoGame>().HasData(
                new VideoGame
                {
                    Id = 1,
                    Title = "The Legend of Zelda: Tears of the Kingdom",
                    Developer = "Nintendo EPD",
                    Publisher = "Nintendo",
                    ReleaseDate = new DateOnly(2023, 5, 12),
                    Genre = "Action-Adventure",
                    Price = 69.99m,
                    Platform = "Nintendo Switch",
                    MetacriticScore = 98,
                    Description = "An epic action-adventure game featuring Link in his most ambitious adventure yet.",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new VideoGame
                {
                    Id = 2,
                    Title = "Baldur's Gate 3",
                    Developer = "Larian Studios",
                    Publisher = "Larian Studios",
                    ReleaseDate = new DateOnly(2023, 8, 3),
                    Genre = "RPG",
                    Price = 59.99m,
                    Platform = "PC",
                    MetacriticScore = 96,
                    Description = "A story-rich, party-based RPG set in the world of D&D.",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
