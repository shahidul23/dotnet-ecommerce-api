using System;
using System.Security.Cryptography.X509Certificates;
using dotnet_ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_ecommerce_api.data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {  }
    public DbSet<Category> Categories{ get; set;} 
    public DbSet<Product> Products {get; set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasKey(c => c.CategortId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategortId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
}
