using Domain.User;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.JwtServices
{
    public class JwtService : IJwtService
    {
        public string Generate(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes("MySecretKey123456789qazwsxedc!@##$%^^&*()"); // must be greater than 256 bytes
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var claims = _getClaims(user);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = "MyApp",
                Audience = "MyApp",
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var jwt = tokenHandler.WriteToken(securityToken);

            return jwt;
        }

        private IEnumerable<Claim> _getClaims(User user)
        {
            //JwtRegisteredClaimNames.Sub

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var roles = new UserRole[] { new UserRole { RoleName = "Admin" } };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));

            return claims;
        }
    }
}
