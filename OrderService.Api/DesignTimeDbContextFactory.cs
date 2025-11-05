using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OrderService.Api.Data;

namespace OrderService.Api
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
	{
		public OrderDbContext CreateDbContext(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false)
				.AddJsonFile("appsettings.Development.json", optional: true)
				.AddEnvironmentVariables()
				.Build();

			var conn = config.GetConnectionString("DefaultConnection");
			var options = new DbContextOptionsBuilder<OrderDbContext>()
				.UseSqlServer(conn)
				.Options;

			return new OrderDbContext(options);
		}
	}
}


