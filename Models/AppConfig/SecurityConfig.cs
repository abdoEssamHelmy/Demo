using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.AppConfig
{
    public class SecurityConfig
    {
        public string Salt { get; set; }
        public string TokenIssuer { get; set; }
        public string TokenAudience { get; set; }
        public string SigningKey { get; set; }
        public int ExpiryInMinutes { get; set; }

        public SecurityKey SecurityKey
        {
            get
            {
                return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
            }
        }
        public SigningCredentials SigningCredentials => new(SecurityKey, SecurityAlgorithms.HmacSha256);
    }
}
