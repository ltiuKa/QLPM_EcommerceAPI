using Microsoft.EntityFrameworkCore;
using ProductService.Api.Models;

namespace ProductService.Api.Data
{
	public class ProductDbContext : DbContext
	{
		public ProductDbContext(DbContextOptions<ProductDbContext> options)
			: base(options)
		{
		}

		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<Category> Categories { get; set; } = null!;
		public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
		public DbSet<ProductImage> ProductImages { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Product
			modelBuilder.Entity<Product>(entity =>
			{
				entity.HasKey(p => p.Id);
				entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
				entity.Property(p => p.ShortDescription).HasMaxLength(500);
				entity.HasOne(p => p.Category)
					  .WithMany(c => c.Products)
					  .HasForeignKey(p => p.CategoryId)
					  .OnDelete(DeleteBehavior.SetNull);

				entity.HasMany(p => p.Variants)
					  .WithOne(v => v.Product)
					  .HasForeignKey(v => v.ProductId)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasMany(p => p.Images)
					  .WithOne(i => i.Product)
					  .HasForeignKey(i => i.ProductId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// Category
			modelBuilder.Entity<Category>(entity =>
			{
				entity.HasKey(c => c.Id);
				entity.Property(c => c.Name).IsRequired().HasMaxLength(150);
			});

			// Variant
			modelBuilder.Entity<ProductVariant>(entity =>
			{
				entity.HasKey(v => v.Id);
				entity.Property(v => v.Sku).HasMaxLength(200);
				entity.Property(v => v.BasePrice).HasColumnType("decimal(18,2)");
			});

			// Image
			modelBuilder.Entity<ProductImage>(entity =>
			{
				entity.HasKey(i => i.Id);
				entity.Property(i => i.Url).IsRequired();
			});

			// VariantPrice
			modelBuilder.Entity<VariantPrice>(entity =>
			{
				entity.HasKey(p => p.Id);
				entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
			});
		}
	}
}
