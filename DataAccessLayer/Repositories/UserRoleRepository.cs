using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data;
using DataAccessLayer.Repositories.Interfaces;
namespace DataAccessLayer.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(DBContext context) : base(context)
        {
        }

    }
}
