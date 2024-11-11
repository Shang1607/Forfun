using System;
using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data;

public class ApplicationDBContext: DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options)
    {
       // Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; } // migrate users
    public DbSet<Product> Products { get; set; } // migrate products

    public DbSet<Category> Categories { get; set; } // migrerer categories
    public DbSet<ProductCategory> ProductCategories { get; set; } // migrerer productcategories for mange til mange relasjon

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurere mange-til-mange-relasjonen
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);

            base.OnModelCreating(modelBuilder);
        }
}
