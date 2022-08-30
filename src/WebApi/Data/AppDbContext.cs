using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.DomainModels;
using WebApi.Seeds;

namespace WebApi.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Customer>().HasData(CustomerDataSeed.Customers());
        base.OnModelCreating(builder);
    }

    public DbSet<Customer> Customers => Set<Customer>();
}