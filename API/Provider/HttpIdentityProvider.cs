using Microsoft.AspNetCore.Http;
using Models.Providers;
using Models.View;
using System.Security.Claims;

namespace API.Provider
{
    public class HttpIdentityProvider : IIdentityProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HttpIdentityProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public CurrentLogin Login
        {
            get
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    ClaimsIdentity Claims = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                    return new CurrentLogin()
                    {
                        Id = int.Parse(Claims.FindFirst("__Id").Value),
                        MerchantId = int.Parse(Claims.FindFirst("__MerchantId").Value)
                    };
                }
                return null;
            }
        }
    }
}
