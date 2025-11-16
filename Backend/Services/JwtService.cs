using Backend.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly int _expirationDays;

        public JwtService()
        {
            _secretKey = "Flk11i5cNhR0BHhsWFzW0DWtmcFKfwwvj1ioJTnIo7uRXyIdU2XnLsXipdPbCodg!";
            _expirationDays = 7;
        }

        // Accept UserRole enum instead of string
        public string GenerateToken(int userId, string email, UserRole role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role.ToString()) // ← Enum to string
                }),
                Expires = DateTime.UtcNow.AddDays(_expirationDays),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        // Helper to get UserRole from token claims
        public UserRole? GetRoleFromToken(ClaimsPrincipal principal)
        {
            var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim))
                return null;

            if (Enum.TryParse<UserRole>(roleClaim, out var role))
                return role;

            return null;
        }
    }
}