using AutoMapper;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.General;
using Models.Providers;
using Models.View;
using Models.View.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        private readonly IIdentityProvider _identityProvider;
       
        public UserController(IUserService userService, IMapper mapper, IIdentityProvider identityProvider)
        {
            _userService = userService;
            _mapper = mapper;
            _identityProvider = identityProvider;
        }

        [HttpPost]
        [Route("login")]
        public async Task<GResponse<LoginResponse>> Login(LoginRequest LoginRequest)
        {
            return await _userService.Login(LoginRequest);
        }

        [HttpPost]
        [Route("Search")]
        [Authorize(Roles = "Admin")]
        public async Task<List<VmUser>> Search(GeneralSearchRequest SearchRequest)
        {
            return _mapper.Map<List<VmUser>>(await _userService.Search(SearchRequest.Value));
        }

    }
}
