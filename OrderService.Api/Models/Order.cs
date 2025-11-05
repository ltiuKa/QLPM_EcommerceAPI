namespace OrderService.Api.Models;

public class Order
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }           // buyer (AuthService)
	public DateTime OrderDate { get; set; } = DateTime.UtcNow;
	public decimal TotalAmount { get; set; }
	public string Status { get; set; } = "Pending";
	public string? ShippingAddress { get; set; }
	public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
	public PaymentInfo? Payment { get; set; }
}
