using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthService.Core.DTOs;
using AuthService.Core.Settings;
using AuthService.UseCases.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Auth
{
    public class JwtTokenGenerator: ITokenGenerator
    {
        private readonly JwtSettings _settings;
        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            this._settings = options.Value;
        }

        public string GenerateToken(RegisterRequest dto)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,dto.Name),
                new Claim(JwtRegisteredClaimNames.PhoneNumber,dto.Phone.Number),
                new Claim("name",dto.Name),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiresInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
  }
}