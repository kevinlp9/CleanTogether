using CleanTogether.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanTogether.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<CleaningEvent> CleaningEvents { get; set; }
    public DbSet<EventAttendee> EventAttendees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relación entre EventAttendee y CleaningEvent
        modelBuilder.Entity<EventAttendee>()
            .HasOne(ea => ea.CleaningEvent)
            .WithMany(ce => ce.EventAttendees)
            .HasForeignKey(ea => ea.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}