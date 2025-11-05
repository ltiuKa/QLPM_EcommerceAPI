namespace ProductService.Api.Models;

public class ProductVariant
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public Product? Product { get; set; }

	// e.g. SKU: "IP14-128GB-BLK", attributes combined as JSON/text or normalized table
	public string Sku { get; set; } = string.Empty;

	// Human readable attributes (could be structured in real project)
	public string? Attributes { get; set; } // e.g. "Color:Black;RAM:8GB;Storage:128GB"

	// Base price for this variant (can be overridden by price tiers)
	public decimal BasePrice { get; set; }

	// Stock for this variant
	public int Stock { get; set; }

	// Optional image specific to variant
	public string? ImageUrl { get; set; }

	// Optional collection of price tiers (e.g., wholesale, promo price by qty)
	public ICollection<VariantPrice>? Price { get; set; }

	public ICollection<ProductImage>? Images { get; set; }


}
