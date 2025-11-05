namespace AuthService.Api.Models
{
	public class User
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public string Role { get; set; } = "User"; // Admin | User | Seller
		public string? AvatarUrl { get; set; }      // link ảnh đại diện
		public string? PhoneNumber { get; set; }    // số điện thoại
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
