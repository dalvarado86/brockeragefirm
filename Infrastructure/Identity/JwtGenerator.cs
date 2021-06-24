using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity
{
    /// <summary>
    /// JwtGenerator.
    /// </summary>
    public class JwtGenerator : IJwtGenerator
    {
        private readonly int _expiration;
        private readonly SymmetricSecurityKey _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtGenerator"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public JwtGenerator(IConfiguration configuration)
        {
            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Token:Key"]));
            _expiration = Convert.ToInt32(configuration["Token:ExpireHours"]);
        }

        /// <inheritdoc/>
        public string CreateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // Generate signing credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(_expiration),
                SigningCredentials = creds
            };

            var tokenHandle = new JwtSecurityTokenHandler();
            var token = tokenHandle.CreateToken(tokenDescriptor);
            return tokenHandle.WriteToken(token);
        }
    }
}
