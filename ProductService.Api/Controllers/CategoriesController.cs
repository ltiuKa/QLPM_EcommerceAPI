using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Data;
using ProductService.Api.Models;

namespace ProductService.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriesController : ControllerBase
	{
		private readonly ProductDbContext _db;
		public CategoriesController(ProductDbContext db) => _db = db;

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var items = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
			return Ok(items);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> Get(Guid id)
		{
			var cat = await _db.Categories.FindAsync(id);
			if (cat == null) return NotFound();
			return Ok(cat);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Category cat)
		{
			cat.Id = Guid.NewGuid();
			_db.Categories.Add(cat);
			await _db.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { id = cat.Id }, cat);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] Category input)
		{
			var ex = await _db.Categories.FindAsync(id);
			if (ex == null) return NotFound();
			ex.Name = input.Name;
			ex.Description = input.Description;
			await _db.SaveChangesAsync();
			return NoContent();
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var ex = await _db.Categories.FindAsync(id);
			if (ex == null) return NotFound();
			_db.Categories.Remove(ex);
			await _db.SaveChangesAsync();
			return NoContent();
		}
	}
}


