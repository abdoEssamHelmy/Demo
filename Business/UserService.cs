using AutoMapper;
using Business.Interfaces;
using DataAccessLayer.Interfaces;
using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly ICryptographyService CryptographyService;

        private readonly IMapper Mapper;
        public UserService(IUnitOfWork UnitOfWork, ICryptographyService CryptographyService, IMapper Mapper)
        {
            this.UnitOfWork = UnitOfWork;
            this.CryptographyService = CryptographyService;
            this.Mapper = Mapper;
        }
        public async Task<GResponse<LoginResponse>> Login(LoginRequest Request)
        {
            
            var User = await UnitOfWork.UserRepository.GetLogin(Request.UserName, Request.Password);
            if(User == null)
                return GResponse<LoginResponse>.CreateFailure("InvalidLogin");
            try
            {
                var userRR = await UnitOfWork.UserRoleRepository.GetAllAsync();
            }
            catch(Exception ex)
            {

            }
            #region Generate Claims
            List<Claim> Claims = new List<Claim>();
            foreach (var UserRoles in User.UserRoles)
            {
                Claims.Add(new Claim(ClaimTypes.Role, UserRoles.Role.Name));
            }

            Claims.Add(new Claim(ClaimTypes.Name, User.UserName));
            Claims.Add(new Claim("__Id", $"{User.Id}"));
            #endregion
            return GResponse<LoginResponse>.Create(new LoginResponse()
            {
                Token = CryptographyService.GenerateJwt(Claims),
                Login = Mapper.Map<CurrentLogin>(User)
            }, "Success");            

        }
    }
}
