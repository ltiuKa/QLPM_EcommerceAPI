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
	public class CartController : ControllerBase
	{
		private readonly OrderDbContext _db;
		public CartController(OrderDbContext db) => _db = db;

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var sub = User.FindFirstValue("sub");
			if (!Guid.TryParse(sub, out var userId)) return Unauthorized();
			var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
			if (cart == null)
			{
				cart = new Cart { Id = Guid.NewGuid(), UserId = userId };
				_db.Carts.Add(cart);
				await _db.SaveChangesAsync();
			}
			return Ok(cart);
		}

		public record AddItemRequest(Guid ProductId, Guid? ProductVariantId, int Quantity, decimal UnitPrice);

		[Authorize]
		[HttpPost("items")]
		public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
		{
			var sub = User.FindFirstValue("sub");
			if (!Guid.TryParse(sub, out var userId)) return Unauthorized();
			var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId) 
				?? new Cart { Id = Guid.NewGuid(), UserId = userId };
			if (cart.Id == Guid.Empty) cart.Id = Guid.NewGuid();
			if (_db.Entry(cart).State == EntityState.Detached) _db.Carts.Add(cart);

			var ex = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId && i.ProductVariantId == request.ProductVariantId);
			if (ex == null)
			{
				cart.Items.Add(new CartItem
				{
					Id = Guid.NewGuid(),
					CartId = cart.Id,
					ProductId = request.ProductId,
					ProductVariantId = request.ProductVariantId,
					Quantity = request.Quantity,
					UnitPrice = request.UnitPrice
				});
			}
			else
			{
				ex.Quantity += request.Quantity;
				ex.UnitPrice = request.UnitPrice; // update snapshot price
			}

			await _db.SaveChangesAsync();
			return Ok(cart);
		}

		[Authorize]
		[HttpDelete("items/{itemId:guid}")]
		public async Task<IActionResult> RemoveItem(Guid itemId)
		{
			var sub = User.FindFirstValue("sub");
			if (!Guid.TryParse(sub, out var userId)) return Unauthorized();
			var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
			if (cart == null) return NotFound();
			var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
			if (item == null) return NotFound();
			_db.CartItems.Remove(item);
			await _db.SaveChangesAsync();
			return NoContent();
		}
	}
}


