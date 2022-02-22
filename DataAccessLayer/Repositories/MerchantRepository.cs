using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data;
using DataAccessLayer.Repositories.Interfaces;
namespace DataAccessLayer.Repositories
{
    public class MerchantRepository : GenericRepository<Merchant>, IMerchantRepository
    {
        public MerchantRepository(DBContext context) : base(context)
        {
        }

    }
}
