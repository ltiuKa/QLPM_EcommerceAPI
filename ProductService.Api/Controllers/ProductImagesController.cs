using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Data;
using ProductService.Api.Models;

namespace ProductService.Api.Controllers
{
	[ApiController]
	[Route("api/products/{productId:guid}/images")]
	public class ProductImagesController : ControllerBase
	{
		private readonly ProductDbContext _db;
		private readonly IWebHostEnvironment _env;
		public ProductImagesController(ProductDbContext db, IWebHostEnvironment env)
		{
			_db = db; _env = env;
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[RequestSizeLimit(20_000_000)] // 20MB
		public async Task<IActionResult> Upload(Guid productId, IFormFile file, [FromQuery] bool isMain = false)
		{
			var product = await _db.Products.FindAsync(productId);
			if (product == null) return NotFound(new { message = "Product not found" });
			if (file == null || file.Length == 0) return BadRequest(new { message = "File trống" });

			var imagesDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "images", productId.ToString());
			Directory.CreateDirectory(imagesDir);
			var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
			var fullPath = Path.Combine(imagesDir, fileName);
			await using (var stream = System.IO.File.Create(fullPath))
			{
				await file.CopyToAsync(stream);
			}

			var relativeUrl = $"/images/{productId}/{fileName}";
			if (isMain)
			{
				product.MainImageUrl = relativeUrl;
			}

			var img = new ProductImage
			{
				Id = Guid.NewGuid(),
				ProductId = productId,
				Url = relativeUrl,
				IsMain = isMain,
				SortOrder = 0,
				CreatedAt = DateTime.UtcNow
			};
			_db.ProductImages.Add(img);
			await _db.SaveChangesAsync();

			return Created(relativeUrl, img);
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{imageId:guid}")]
		public async Task<IActionResult> Delete(Guid productId, Guid imageId)
		{
			var img = await _db.ProductImages.FirstOrDefaultAsync(x => x.Id == imageId && x.ProductId == productId);
			if (img == null) return NotFound();
			_db.ProductImages.Remove(img);
			await _db.SaveChangesAsync();
			return NoContent();
		}
	}
}


