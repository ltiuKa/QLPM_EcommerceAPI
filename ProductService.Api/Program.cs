using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ProductService.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// OpenAPI / Swagger (giữ theo project của bạn)
builder.Services.AddOpenApi();

// Add controllers (nếu bạn sẽ thêm controller)
builder.Services.AddControllers().AddJsonOptions(o =>
{
	o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// CORS dev: cho phép tất cả (đơn giản chạy nhanh)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// phục vụ static files cho ảnh
app.UseStaticFiles();

app.MapControllers(); // map API controllers

// Seed dữ liệu demo nhanh
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
	await db.Database.EnsureCreatedAsync();
	if (!await db.Products.AnyAsync())
	{
		var phoneCat = new ProductService.Api.Models.Category { Id = Guid.NewGuid(), Name = "Điện thoại" };
		db.Categories.Add(phoneCat);

		var iphone = new ProductService.Api.Models.Product
		{
			Id = Guid.NewGuid(),
			Name = "iPhone 15 128GB",
			ShortDescription = "iPhone 15, 128GB, chính hãng",
			CategoryId = phoneCat.Id,
			CreatedAt = DateTime.UtcNow,
			MainImageUrl = "https://picsum.photos/seed/iphone15/600/400"
		};
		iphone.Variants.Add(new ProductService.Api.Models.ProductVariant
		{
			Id = Guid.NewGuid(),
			ProductId = iphone.Id,
			Sku = "IP15-128-BLK",
			Attributes = "Color:Black;Storage:128GB",
			BasePrice = 21990000,
			Stock = 50
		});
		iphone.Images.Add(new ProductService.Api.Models.ProductImage
		{
			Id = Guid.NewGuid(),
			ProductId = iphone.Id,
			Url = iphone.MainImageUrl!,
			IsMain = true,
			SortOrder = 0
		});

		var galaxy = new ProductService.Api.Models.Product
		{
			Id = Guid.NewGuid(),
			Name = "Samsung Galaxy S23 256GB",
			ShortDescription = "Galaxy S23, 256GB, chính hãng",
			CategoryId = phoneCat.Id,
			CreatedAt = DateTime.UtcNow,
			MainImageUrl = "https://picsum.photos/seed/galaxys23/600/400"
		};
		galaxy.Variants.Add(new ProductService.Api.Models.ProductVariant
		{
			Id = Guid.NewGuid(),
			ProductId = galaxy.Id,
			Sku = "S23-256-GRN",
			Attributes = "Color:Green;Storage:256GB",
			BasePrice = 18990000,
			Stock = 80
		});
		galaxy.Images.Add(new ProductService.Api.Models.ProductImage
		{
			Id = Guid.NewGuid(),
			ProductId = galaxy.Id,
			Url = galaxy.MainImageUrl!,
			IsMain = true,
			SortOrder = 0
		});

		db.Products.AddRange(iphone, galaxy);
		await db.SaveChangesAsync();
	}
}

app.Run();
