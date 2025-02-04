using Microsoft.EntityFrameworkCore;
using PagFilOrd.bd.models;

namespace PagFilOrd.bd;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasOne(e => e.Brand)
            .WithMany(e => e.Categories)
            .HasForeignKey(e => e.BrandId)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<Product>()
            .HasOne(e => e.Category)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.CategoryId)
            .HasPrincipalKey(e => e.Id);
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Brand> Brands { get; set; }
}