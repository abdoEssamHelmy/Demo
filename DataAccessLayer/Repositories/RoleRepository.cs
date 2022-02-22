using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data;
using DataAccessLayer.Repositories.Interfaces;
namespace DataAccessLayer.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(DBContext context) : base(context)
        {
        }
    }
}
