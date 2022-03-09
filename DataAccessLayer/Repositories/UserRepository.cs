using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DBContext context) : base(context)
        {
        }

        public async Task<User> GetLogin(string UserName, string Password)
        {
            var user= await FirstOrDefaultAsync(r => r.UserName == UserName && r.Password == Password);
            var roles = user.UserRoles;
            return user;
        }

        public async Task<List<User>> Search(string SearchValue)
        {
            return await Where(r => r.UserName.Contains(SearchValue)).ToListAsync();
        }
    }
}
