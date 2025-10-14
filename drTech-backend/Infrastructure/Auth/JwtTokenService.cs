using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace drTech_backend.Infrastructure.Auth
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Domain.Entities.User user);
        Domain.Entities.RefreshToken GenerateRefreshToken();
        ClaimsPrincipal? ValidateExpiredToken(string token, string key);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(Domain.Entities.User user)
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("JWT key not configured.");
            
            var creds = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.TryParse(_configuration["Jwt:AccessTokenMinutes"], out var m) ? m : 15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Domain.Entities.RefreshToken GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return new Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(bytes),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(int.TryParse(_configuration["Jwt:RefreshTokenDays"], out var d) ? d : 7)
            };
        }

        public ClaimsPrincipal? ValidateExpiredToken(string token, string key)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = false
            };

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (securityToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}


