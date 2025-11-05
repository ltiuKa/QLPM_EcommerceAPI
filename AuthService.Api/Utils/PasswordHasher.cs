using System.Security.Cryptography;
using System.Text;

namespace AuthService.Api.Utils
{
	public static class PasswordHasher
	{
		public static (string Hash, string Salt) HashPassword(string password)
		{
			var saltBytes = RandomNumberGenerator.GetBytes(16);
			var salt = Convert.ToBase64String(saltBytes);
			using var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
			var hash = Convert.ToBase64String(deriveBytes.GetBytes(32));
			return (hash, salt);
		}

		public static bool VerifyPassword(string password, string base64Hash, string base64Salt)
		{
			var saltBytes = Convert.FromBase64String(base64Salt);
			using var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
			var computed = Convert.ToBase64String(deriveBytes.GetBytes(32));
			return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(computed), Encoding.UTF8.GetBytes(base64Hash));
		}
	}
}


