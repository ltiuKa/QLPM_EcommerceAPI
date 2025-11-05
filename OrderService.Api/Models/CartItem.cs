namespace OrderService.Api.Models;

public class CartItem
{
	public Guid Id { get; set; }
	public Guid CartId { get; set; }
	public Cart? Cart { get; set; }

	public Guid ProductId { get; set; }         // ID product (từ ProductService)
	public Guid? ProductVariantId { get; set; } // nếu có variant
	public int Quantity { get; set; }
	public decimal UnitPrice { get; set; }      // snapshot price when added
	public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
