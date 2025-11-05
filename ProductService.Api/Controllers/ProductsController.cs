using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Data;
using ProductService.Api.Models;

namespace ProductService.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly ProductDbContext _db;
		public ProductsController(ProductDbContext db) => _db = db;

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var products = await _db.Products
									.Include(p => p.Images)
									.Include(p => p.Variants)
									.Include(p => p.Category)
									.ToListAsync();
			return Ok(products);
		}

		[HttpGet("search")]
		public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] Guid? categoryId)
		{
			var query = _db.Products
				.Include(p => p.Images)
				.Include(p => p.Variants)
				.Include(p => p.Category)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(q))
			{
				var kw = q.Trim();
				query = query.Where(p => p.Name.Contains(kw) || (p.ShortDescription != null && p.ShortDescription.Contains(kw)));
			}

			if (categoryId.HasValue)
			{
				query = query.Where(p => p.CategoryId == categoryId);
			}

			var items = await query.OrderByDescending(p => p.CreatedAt).Take(50).ToListAsync();
			return Ok(items);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> Get(Guid id)
		{
			var product = await _db.Products
								   .Include(p => p.Images)
								   .Include(p => p.Variants)
								   .Include(p => p.Category)
								   .FirstOrDefaultAsync(p => p.Id == id);
			if (product == null) return NotFound();
			return Ok(product);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Product product)
		{
			product.Id = Guid.NewGuid();
			product.CreatedAt = DateTime.UtcNow;
			_db.Products.Add(product);
			await _db.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] Product updated)
		{
			var ex = await _db.Products.FindAsync(id);
			if (ex == null) return NotFound();

			ex.Name = updated.Name;
			ex.ShortDescription = updated.ShortDescription;
			ex.LongDescription = updated.LongDescription;
			ex.MainImageUrl = updated.MainImageUrl;
			ex.CategoryId = updated.CategoryId;

			await _db.SaveChangesAsync();
			return NoContent();
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var ex = await _db.Products.FindAsync(id);
			if (ex == null) return NotFound();

			_db.Products.Remove(ex);
			await _db.SaveChangesAsync();
			return NoContent();
		}
	}
}
