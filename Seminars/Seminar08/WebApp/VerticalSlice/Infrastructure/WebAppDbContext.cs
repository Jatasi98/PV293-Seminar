using Microsoft.EntityFrameworkCore;
using VerticalSlice.Domain.Entities;

namespace VerticalSlice.Infrastructure;

public class WebAppDbContext(DbContextOptions<WebAppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<CartItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);

        modelBuilder.Entity<Category>().Property(e => e.CreatedOnUTC).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Product>().Property(e => e.CreatedOnUTC).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Customer>().Property(e => e.CreatedOnUTC).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Cart>().Property(e => e.CreatedOnUTC).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<CartItem>().Property(e => e.CreatedOnUTC).HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Cart)
            .WithOne(cart => cart.Customer)
            .HasForeignKey<Cart>(cart => cart.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.CustomerId)
            .IsUnique();

        modelBuilder.Entity<Cart>()
            .HasMany(c => c.Items)
            .WithOne(i => i.Cart)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, CreatedOnUTC = seedTime, Name = "Electronics", Description = "Headphones, laptops & more" },
            new Category { Id = 2, CreatedOnUTC = seedTime, Name = "Accessories", Description = "Keyboards, mice, cables" },
            new Category { Id = 3, CreatedOnUTC = seedTime, Name = "Home", Description = "Kitchen & decor" },
            new Category { Id = 4, CreatedOnUTC = seedTime, Name = "Sale", Description = "Limited time deals" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, CreatedOnUTC = seedTime, Name = "Wireless Headphones", Price = 129.99m, CategoryId = 1, Description = "Comfortable over-ear, 30h battery." },
            new Product { Id = 2, CreatedOnUTC = seedTime, Name = "Ultrabook 13\"", Price = 999.00m, CategoryId = 1, Description = "Lightweight laptop for work & travel." },
            new Product { Id = 3, CreatedOnUTC = seedTime, Name = "Mechanical Keyboard", Price = 79.90m, CategoryId = 2, Description = "Tactile switches, RGB backlight." },
            new Product { Id = 4, CreatedOnUTC = seedTime, Name = "Ergo Mouse", Price = 39.90m, CategoryId = 2, Description = "Ergonomic, multi-device pairing." },
            new Product { Id = 5, CreatedOnUTC = seedTime, Name = "Chef’s Knife", Price = 49.50m, CategoryId = 3, Description = "8-inch stainless steel blade." },
            new Product { Id = 6, CreatedOnUTC = seedTime, Name = "Pour-over Kettle", Price = 35.00m, CategoryId = 3, Description = "Precision spout, stovetop safe." },
            new Product { Id = 7, CreatedOnUTC = seedTime, Name = "USB-C Cable (2m)", Price = 12.99m, CategoryId = 4, Description = "60W charge, braided, on sale." },
            new Product { Id = 8, CreatedOnUTC = seedTime, Name = "Bluetooth Speaker", Price = 59.00m, CategoryId = 4, Description = "IPX5 splash-proof, 12h playtime." }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, CreatedOnUTC = seedTime, FirstName = "Demo", LastName = "Customer", Email = "demo@example.com" }
        );
    }
}