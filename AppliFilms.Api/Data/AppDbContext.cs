using Microsoft.EntityFrameworkCore;
using AppliFilms.Api.Entities;

namespace AppliFilms.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User?> Users { get; set; }
        public DbSet<Movie?> Movies { get; set; }
        public DbSet<Request?> Requests { get; set; }
        public DbSet<Approval?> Approvals { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Request>()
                .HasIndex(r => new { r.MovieId, r.EventDate })
                .IsUnique();

            modelBuilder.Entity<Approval>()
                .HasIndex(a => new { a.RequestId, a.UserId })
                .IsUnique();
            
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties()
                             .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
                {
                    property.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }
        }
    }
}