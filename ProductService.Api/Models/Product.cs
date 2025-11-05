namespace ProductService.Api.Models
{
	public class Product
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string ShortDescription { get; set; } = string.Empty;
		public string? LongDescription { get; set; }
		public Guid? CategoryId { get; set; }
		public Category? Category { get; set; }
		public string? MainImageUrl { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Relationship: 1 product -> many variants (color/ram/storage...)
		public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
		public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

	}
}
