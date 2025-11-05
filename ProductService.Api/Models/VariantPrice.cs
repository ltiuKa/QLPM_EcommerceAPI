namespace ProductService.Api.Models;

public class VariantPrice
{
	public Guid Id { get; set; }
	public Guid ProductVariantId { get; set; }
	public ProductVariant? ProductVariant { get; set; }

	// e.g. "Retail", "Wholesale", "Promo"
	public string PriceType { get; set; } = "Retail";

	// price for this type
	public decimal Price { get; set; }

	// optional: min quantity for this tier (e.g., wholesale if qty >= 10)
	public int? MinQuantity { get; set; }
}
