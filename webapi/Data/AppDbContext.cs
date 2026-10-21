using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using webapi.Entities;
namespace webapi.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<User> AccountUsers { get; set;}
    public DbSet<Employee> Employees { get; set;}
    

}
