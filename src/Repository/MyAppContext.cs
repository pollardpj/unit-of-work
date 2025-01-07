using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MyAppContext(DbContextOptions<MyAppContext> _options) : DbContext(_options)
{
    public DbSet<Order> Orders { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .ToTable("Order")
            .Property("Id").ValueGeneratedNever();

        modelBuilder.Entity<Order>()
            .Property(o => o.ProductName)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Order>()
            .Property(o => o.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.RowVersion)
            .IsRowVersion()
            .IsRequired();

        modelBuilder.Entity<OrderEvent>()
            .ToTable("OrderEvent")
            .Property("Id").ValueGeneratedNever();

        modelBuilder.Entity<OrderEvent>()
            .Property(o => o.Payload)
            .IsRequired();

        modelBuilder.Entity<OrderEvent>()
            .Property(o => o.OrderId)
            .IsRequired();

        modelBuilder.Entity<OrderEvent>()
            .Property(o => o.RowVersion)
            .IsRowVersion()
            .IsRequired();
    }
}