using Models.Data;
using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<GResponse<LoginResponse>> Login(LoginRequest Request);
        Task<List<User>> Search(string SearchValue);
    }
}
