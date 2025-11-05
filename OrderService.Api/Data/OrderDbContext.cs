using Microsoft.EntityFrameworkCore;
using OrderService.Api.Models;

namespace OrderService.Api.Data
{
	public class OrderDbContext : DbContext
	{
		public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

		public DbSet<Order> Orders { get; set; } = null!;
		public DbSet<OrderItem> OrderItems { get; set; } = null!;
		public DbSet<Cart> Carts { get; set; } = null!;
		public DbSet<CartItem> CartItems { get; set; } = null!;
		public DbSet<PaymentInfo> Payments { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Order>(e =>
			{
				e.HasKey(o => o.Id);
				e.HasMany(o => o.Items)
					.WithOne(i => i.Order)
					.HasForeignKey(i => i.OrderId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<OrderItem>(e =>
			{
				e.HasKey(i => i.Id);
				e.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
			});

			modelBuilder.Entity<Cart>(e =>
			{
				e.HasKey(c => c.Id);
				e.HasMany(c => c.Items)
					.WithOne(i => i.Cart)
					.HasForeignKey(i => i.CartId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<CartItem>(e =>
			{
				e.HasKey(i => i.Id);
				e.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
			});

			modelBuilder.Entity<PaymentInfo>(e =>
			{
				e.HasKey(p => p.Id);
				e.Property(p => p.Amount).HasColumnType("decimal(18,2)");
			});
		}
	}
}


