using AppliFilms.Api.DTOs.Auth;
using AppliFilms.Api.Entities;
using AppliFilms.Api.Repositories.Interfaces;
using AppliFilms.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppliFilms.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new Exception("Email déjà utilisé");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                DisplayName = dto.DisplayName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                DisplayName = user.DisplayName,
                IsAdmin = user.IsAdmin
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Identifiants invalides");

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                DisplayName = user.DisplayName,
                IsAdmin = user.IsAdmin
            };
        }

        private string GenerateJwtToken(User user)
        {
            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secret))
                throw new Exception("JWT secret non configuré");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("displayName", user.DisplayName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var expires = DateTime.UtcNow.AddHours(1); // token valable 1 heure

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}