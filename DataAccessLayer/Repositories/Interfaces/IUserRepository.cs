using DataAccessLayer.Interfaces;
using Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetLogin(string UserName, string Password);
        Task<List<User>> Search(string SearchValue);
    }
}
