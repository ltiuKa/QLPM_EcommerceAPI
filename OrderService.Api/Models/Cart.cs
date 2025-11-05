namespace OrderService.Api.Models;

public class Cart
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }          // liên kết tới AuthService user
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

	// Tổng tiền tạm tính (tính khi cần, không lưu cố định nếu muốn realtime)
	public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);
}
