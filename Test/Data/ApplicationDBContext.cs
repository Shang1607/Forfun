using System;
using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data;

public class ApplicationDBContext: DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Category> Categories { get; set; }  // migrates categories

    public DbSet<User> Users { get; set; } // migrate users

     public DbSet<Product> Products { get; set; } // migrate products
}
