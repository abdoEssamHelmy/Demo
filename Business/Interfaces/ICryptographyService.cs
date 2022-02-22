using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ICryptographyService
    {
        string CreateSHA256Signature(string Salt, string Data);
        string CreateSHA256Signature(string Data);
        string RefreshJwt();
        string GenerateJwt(List<Claim> Claims);
    }
}
