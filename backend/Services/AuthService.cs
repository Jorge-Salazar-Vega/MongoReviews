using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoReviews.Api.Models;
using MongoReviews.Api.Models.Dtos;

namespace MongoReviews.Api.Services;

public class AuthService
{
    private readonly MongoDbContext _db;
    private readonly JwtSettings _jwt;

    public AuthService(MongoDbContext db, IOptions<JwtSettings> jwt)
    {
        _db = db;
        _jwt = jwt.Value;
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        if (await _db.Users.Find(u => u.Email == request.Email).AnyAsync())
            throw new InvalidOperationException("El email ya está registrado");

        var user = new User
        {
            Nombre = request.Nombre,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _db.Users.InsertOneAsync(user);

        return new AuthResponse
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Token = GenerateToken(user),
            Rol = user.Rol
        };
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await _db.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas");

        return new AuthResponse
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Token = GenerateToken(user),
            Rol = user.Rol
        };
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Rol)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwt.ExpirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class JwtSettings
{
    public string Secret { get; set; } = null!;
    public int ExpirationHours { get; set; } = 24;
}
