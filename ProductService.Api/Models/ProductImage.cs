namespace ProductService.Api.Models;

public class ProductImage
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public Product? Product { get; set; }

	// Optional: if the image belongs to a specific variant (null => product-level)
	public Guid? ProductVariantId { get; set; }
	public ProductVariant? ProductVariant { get; set; }

	public string Url { get; set; } = string.Empty;     // e.g. "/images/products/{productId}/img.jpg"
	public bool IsMain { get; set; } = false;           // ảnh chính
	public int SortOrder { get; set; } = 0;             // thứ tự sắp xếp
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
