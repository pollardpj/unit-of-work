using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MyAppContext(DbContextOptions<MyAppContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .ToTable("Order")
            .HasIndex(o => o.Reference, "UX_Order_Reference")
                .IsUnique();

        modelBuilder.Entity<Order>()
            .Property(o => o.ProductName)
            .IsRequired()
            .HasMaxLength(50);
        
        modelBuilder.Entity<OrderEvent>()
            .ToTable("OrderEvent")
            .HasIndex(e => e.Reference, "UX_OrderEvent_Reference")
                .IsUnique();

        modelBuilder.Entity<OrderEvent>()
            .Property(o => o.Payload)
            .IsRequired();
    }
}