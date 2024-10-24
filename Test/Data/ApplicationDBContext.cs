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

    public DbSet<Category> Categories { get; set; } 

     public DbSet<User> Users { get; set; }
}
