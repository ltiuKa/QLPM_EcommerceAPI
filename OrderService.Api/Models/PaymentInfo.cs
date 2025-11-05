namespace OrderService.Api.Models;

public class PaymentInfo
{
	public Guid Id { get; set; }
	public Guid OrderId { get; set; }
	public string PaymentMethod { get; set; } = "Cash"; // Cash | Card | VNPay|...
	public string? TransactionId { get; set; }
	public DateTime? PaidAt { get; set; }
	public decimal Amount { get; set; }
}
