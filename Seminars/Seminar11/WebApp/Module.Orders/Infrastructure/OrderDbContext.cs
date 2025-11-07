using Microsoft.EntityFrameworkCore;
using Module.Orders.Domain.Entities;

namespace Module.Orders.Infrastructure;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>().Property(e => e.CreatedOnUTC).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<OrderItem>().Property(e => e.CreatedOnUTC).HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Order>()
            .HasIndex(c => c.CustomerId);

        modelBuilder.Entity<Order>()
            .HasMany(c => c.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}