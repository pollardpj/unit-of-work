using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MyAppContext(DbContextOptions<MyAppContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
    public DbSet<Order> Orders { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Order>().ToTable("Order");
    }
}