using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Api.Data;
using OrderService.Api.Models;
using System.Security.Claims;

namespace OrderService.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrdersController : ControllerBase
	{
		private readonly OrderDbContext _db;
		public OrdersController(OrderDbContext db) => _db = db;

		[Authorize]
		[HttpGet("my")]
		public async Task<IActionResult> MyOrders()
		{
			var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
			var sub = User.FindFirstValue("sub");
			var idStr = sub ?? userIdStr;
			if (!Guid.TryParse(idStr, out var userId)) return Unauthorized();

			var orders = await _db.Orders
				.Include(o => o.Items)
				.OrderByDescending(o => o.OrderDate)
				.Where(o => o.UserId == userId)
				.ToListAsync();
			return Ok(orders);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
		{
			var sub = User.FindFirstValue("sub");
			if (!Guid.TryParse(sub, out var userId)) return Unauthorized();

			if (request.Items == null || request.Items.Count == 0)
				return BadRequest(new { message = "Danh sách sản phẩm trống" });

			var order = new Order
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				OrderDate = DateTime.UtcNow,
				Status = "Pending",
				ShippingAddress = request.ShippingAddress
			};

			foreach (var i in request.Items)
			{
				order.Items.Add(new OrderItem
				{
					Id = Guid.NewGuid(),
					OrderId = order.Id,
					ProductId = i.ProductId,
					ProductVariantId = i.ProductVariantId,
					Quantity = i.Quantity,
					UnitPrice = i.UnitPrice
				});
			}

			order.TotalAmount = order.Items.Sum(x => x.UnitPrice * x.Quantity);

			_db.Orders.Add(order);
			await _db.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
		}

		[Authorize]
		[HttpPost("checkout-from-cart")]
		public async Task<IActionResult> CheckoutFromCart([FromBody] string? shippingAddress)
		{
			var sub = User.FindFirstValue("sub");
			if (!Guid.TryParse(sub, out var userId)) return Unauthorized();
			var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
			if (cart == null || cart.Items.Count == 0) return BadRequest(new { message = "Giỏ hàng trống" });

			var order = new Order
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				OrderDate = DateTime.UtcNow,
				Status = "Pending",
				ShippingAddress = shippingAddress
			};

			foreach (var i in cart.Items)
			{
				order.Items.Add(new OrderItem
				{
					Id = Guid.NewGuid(),
					OrderId = order.Id,
					ProductId = i.ProductId,
					ProductVariantId = i.ProductVariantId,
					Quantity = i.Quantity,
					UnitPrice = i.UnitPrice
				});
			}
			order.TotalAmount = order.Items.Sum(x => x.UnitPrice * x.Quantity);

			_db.Orders.Add(order);
			// Xoá giỏ sau khi tạo đơn
			_db.CartItems.RemoveRange(cart.Items);
			await _db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
		}

		[Authorize]
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var sub = User.FindFirstValue("sub");
			if (!Guid.TryParse(sub, out var userId)) return Unauthorized();

			var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
			if (order == null) return NotFound();
			return Ok(order);
		}

		public record CreateOrderRequest(string? ShippingAddress, List<OrderItemRequest> Items);
		public record OrderItemRequest(Guid ProductId, Guid? ProductVariantId, int Quantity, decimal UnitPrice);
	}
}


