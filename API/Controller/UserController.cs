using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View;
using System.Threading.Tasks;

namespace API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService UserService;
        public UserController(IUserService UserService)
        {
            this.UserService = UserService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<GResponse<LoginResponse>> Login(LoginRequest LoginRequest)
        {
            return await UserService.Login(LoginRequest);
        }
    }
}
