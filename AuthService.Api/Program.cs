using AuthService.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// DbContext (đăng ký trước khi Build)
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AuthDbContext>(options =>
	options.UseSqlServer(conn, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory_Auth", "auth")));

// Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS dev cho phép tất cả (tối giản cho MVP)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
		policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-secret-key-change-me";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ECommerce.Auth";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ECommerce.Client";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtIssuer,
			ValidAudience = jwtAudience,
			IssuerSigningKey = signingKey
		};
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure DB created (dev nhanh)
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
	await db.Database.EnsureCreatedAsync();
}

app.Run();
