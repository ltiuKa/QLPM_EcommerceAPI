using AuthService.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthService.Api
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
	{
		public AuthDbContext CreateDbContext(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false)
				.AddJsonFile("appsettings.Development.json", optional: true)
				.AddEnvironmentVariables()
				.Build();

			var conn = config.GetConnectionString("DefaultConnection");
			var options = new DbContextOptionsBuilder<AuthDbContext>()
				.UseSqlServer(conn, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory_Auth", "auth"))
				.Options;

			return new AuthDbContext(options);
		}
	}
}


