using AuthService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Data
{
	public class AuthDbContext : DbContext
	{
		public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
		public DbSet<User> Users { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("auth"); // tạo schema auth
			base.OnModelCreating(modelBuilder);
		}
	}
}
