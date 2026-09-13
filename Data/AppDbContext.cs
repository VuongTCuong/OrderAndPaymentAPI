
using Microsoft.EntityFrameworkCore;
using OrderAndPaymentAPI.Entities;
namespace OrderAndPaymentAPI.Data;
public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<User> User {get; set;}
    public DbSet<Product> Product {get; set;}
    public DbSet<Cart> Cart {get;set;}
    public DbSet<CartItem> CartItem {get; set;}

    public DbSet<RefreshToken> RefreshToken {get; set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}