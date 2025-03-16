using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Models.Task> Tasks => Set<Models.Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Task>()
            .Property(t => t.CreatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<Models.Task>()
            .Property(t => t.UpdatedAt)
            .HasDefaultValueSql("NOW()");
    }
}