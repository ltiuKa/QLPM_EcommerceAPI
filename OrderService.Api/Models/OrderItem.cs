namespace OrderService.Api.Models;

public class OrderItem
{
	public Guid Id { get; set; }
	public Guid OrderId { get; set; }
	public Order? Order { get; set; }

	public Guid ProductId { get; set; }
	public Guid? ProductVariantId { get; set; }
	public int Quantity { get; set; }
	public decimal UnitPrice { get; set; } // snapshot
	public decimal LineTotal => Quantity * UnitPrice;
}

