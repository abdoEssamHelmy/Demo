using DataAccessLayer.Repositories.Interfaces;
using Models.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMerchantRepository MerchantRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        Task<int> CommitAsync();
        //Task<GResponse<T>> TryCommitAsync<T>(T DbRecord);
        DBContext DbContext { get; }
    }
}
