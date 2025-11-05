using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Api.Data;
using AuthService.Api.Models;
using AuthService.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly AuthDbContext _dbContext;
		private readonly IConfiguration _configuration;

		public AuthController(AuthDbContext dbContext, IConfiguration configuration)
		{
			_dbContext = dbContext;
			_configuration = configuration;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest(new { message = "Email và mật khẩu là bắt buộc" });
			}

			var existed = await _dbContext.Users.AnyAsync(u => u.Email == request.Email);
			if (existed)
			{
				return Conflict(new { message = "Email đã tồn tại" });
			}

			var (hash, salt) = PasswordHasher.HashPassword(request.Password);

			var user = new User
			{
				Id = Guid.NewGuid(),
				FullName = request.FullName ?? string.Empty,
				Email = request.Email,
				PasswordHash = $"{hash}:{salt}",
				Role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role
			};

			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();

			return CreatedAtAction(null!, new { id = user.Id }, new { user.Id, user.Email, user.FullName, user.Role });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
			if (user == null)
			{
				return Unauthorized(new { message = "Sai email hoặc mật khẩu" });
			}

			var parts = user.PasswordHash.Split(':');
			if (parts.Length != 2 || !PasswordHasher.VerifyPassword(request.Password, parts[0], parts[1]))
			{
				return Unauthorized(new { message = "Sai email hoặc mật khẩu" });
			}

			var token = GenerateJwtToken(user);
			return Ok(new { access_token = token, token_type = "Bearer", user = new { user.Id, user.Email, user.FullName, user.Role } });
		}

		private string GenerateJwtToken(User user)
		{
			var jwtKey = _configuration["Jwt:Key"] ?? "dev-secret-key-change-me";
			var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ECommerce.Auth";
			var jwtAudience = _configuration["Jwt:Audience"] ?? "ECommerce.Client";

			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
			var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new(JwtRegisteredClaimNames.Email, user.Email),
				new(ClaimTypes.Role, user.Role),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var token = new JwtSecurityToken(
				issuer: jwtIssuer,
				audience: jwtAudience,
				claims: claims,
				expires: DateTime.UtcNow.AddHours(2),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public record RegisterRequest(string Email, string Password, string? FullName, string? Role);
		public record LoginRequest(string Email, string Password);
	}
}


