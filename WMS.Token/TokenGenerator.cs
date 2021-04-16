using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WMS.Entities;

namespace WMS.Token
{
    public static class TokenGenerator
    {
        public static string GenerateTokenJwt(User user)
        {
            //appsetting for token JWT
            string secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            string audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            string issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            string expireTime = ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"];

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim("IdUser", user.IdUser), new Claim("NameUser", user.NameUser), new Claim("FirstName", user.FirstName), new Claim("WhsCode", user.WhsCode)});

            //create token to the user
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials
            );

            return tokenHandler.WriteToken(jwtSecurityToken); ;
        }
    }
}
