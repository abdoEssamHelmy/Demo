using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.View
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public CurrentLogin Login { get; set; }
    }
}
