using Crud.Models;
using Microsoft.EntityFrameworkCore;

namespace Crud.Data;

public class AppDbContext : DbContext
{
    public DbSet<ProductModel> Products { get; set; }
    public DbSet<ItemModel> Items { get; set; }
    public DbSet<CartModel> Carts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemModel>()
            .HasOne(i => i.Product)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CartModel>()
            .HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.ItemsCarrinho)
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder) 
        => optionsBuilder.UseSqlite("DataSource=app.db;Cache=Shared");
}