using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Api.Data;

namespace OrderService.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ReportsController : ControllerBase
	{
		private readonly OrderDbContext _db;
		public ReportsController(OrderDbContext db) => _db = db;

		[Authorize]
		[HttpGet("sales/daily")]
		public async Task<IActionResult> SalesDaily([FromQuery] int days = 7)
		{
			var from = DateTime.UtcNow.Date.AddDays(-Math.Max(1, days) + 1);
			var data = await _db.Orders
				.Where(o => o.OrderDate >= from)
				.GroupBy(o => o.OrderDate.Date)
				.Select(g => new { date = g.Key, revenue = g.Sum(x => x.TotalAmount), orders = g.Count() })
				.OrderBy(x => x.date)
				.ToListAsync();
			return Ok(data);
		}

		[Authorize]
		[HttpGet("top-products")]
		public async Task<IActionResult> TopProducts([FromQuery] int top = 5)
		{
			var data = await _db.OrderItems
				.GroupBy(i => i.ProductId)
				.Select(g => new { productId = g.Key, quantity = g.Sum(x => x.Quantity), revenue = g.Sum(x => x.Quantity * x.UnitPrice) })
				.OrderByDescending(x => x.quantity)
				.Take(Math.Max(1, top))
				.ToListAsync();
			return Ok(data);
		}
	}
}
