using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProductService.Api.Data;

namespace ProductService.Api
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
	{
		public ProductDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ProductDbContext>();
			builder.UseSqlServer("Server=localhost;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True");
			return new ProductDbContext(builder.Options);
		}
	}
}
