using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Models.AppConfig;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Business
{
    public class CryptographyService : ICryptographyService
    {
        private readonly AppConfig _appConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CryptographyService(AppConfig appConfig, IHttpContextAccessor httpContextAccessor)
        {
            this._appConfig = appConfig;
            this._httpContextAccessor = httpContextAccessor;
        }
        public string CreateSHA256Signature(string Data)
        {
            return CreateSHA256Signature(string.Empty, Data);
        }
        public string CreateSHA256Signature(string Salt, string Data)
        {
            StringBuilder Sb = new();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(_appConfig.SecurityConfig.Salt + Salt + Data));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
        public string RefreshJwt()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                ClaimsIdentity ClaimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                List<Claim> Claims = new List<Claim>();
                Claims.AddRange(ClaimsIdentity.FindAll(r => r.Type == ClaimTypes.Role));
                Claims.Add(new Claim(ClaimTypes.Name, ClaimsIdentity.FindFirst(r => r.Type == ClaimTypes.Name).Value));
                Claims.Add(new Claim(ClaimTypes.Email, ClaimsIdentity.FindFirst(r => r.Type == ClaimTypes.Email).Value));
                foreach (var Claim in ClaimsIdentity.FindAll(r => r.Type.StartsWith("__")))
                {
                    Claims.Add(Claim);
                }
                return GenerateJwt(Claims);

            }


            return string.Empty;

        }
        public string GenerateJwt(List<Claim> Claims)
        {
            var claimsIdentity = new ClaimsIdentity();
            foreach (Claim Claim in Claims)
            {
                claimsIdentity.AddClaim(Claim);
            }


            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = _appConfig.SecurityConfig.TokenIssuer,
                Audience = _appConfig.SecurityConfig.TokenAudience,
                SigningCredentials = _appConfig.SecurityConfig.SigningCredentials,
                IssuedAt = now,
                Expires = now.AddMinutes(_appConfig.SecurityConfig.ExpiryInMinutes),
                EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.SecurityConfig.SigningKey)), JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return HttpUtility.UrlDecode(tokenHandler.WriteToken(token));
        }

    }
}
