namespace ProductService.Api.Models;

public class Category
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	// 1 category -> many products
	public ICollection<Product> Products { get; set; } = new List<Product>();
}
