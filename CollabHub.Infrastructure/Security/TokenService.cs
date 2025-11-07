using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CollabHub.Application.Interfaces.Auth;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Configuration;
using CollabHub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CollabHub.Infrastructure.Security
{
    public class TokenService:ITokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly JWTSettings _jwt;
        public TokenService(ApplicationDbContext context,IOptions<JWTSettings> jwt)
        {
            _context = context;
            _jwt = jwt.Value;
        }

        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwt.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Role,user.Role.ToString())
                }),
                Expires = DateTime.Now.AddMinutes(_jwt.AccessTokenExpiresInMinutes),
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public  string GenerateRefreshToken(User user)
        {
            var randomNumber = new Byte[64];
            using (var rng=RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
            
        }

        public bool ValidateToken(string token)
        {
            var refreshtoken = _context.RefreshTokens.FirstOrDefault(t=>t.Token == token);
            if (refreshtoken == null)
                return false;
            if (refreshtoken.ExpiresAt > DateTime.UtcNow && refreshtoken.IsRevoked == false)
                return true;
            return false;
        }

       
    }
}
