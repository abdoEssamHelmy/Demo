using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DataAccessLayer
{
    public class UnitOfWork : IUnitOfWork
    {

       public DBContext DbContext { get; }
       public IUserRepository UserRepository { get; }
        public IMerchantRepository MerchantRepository { get; }
        public IRoleRepository RoleRepository { get; }

        public IUserRoleRepository UserRoleRepository { get; }

        public UnitOfWork(DBContext DbContext,
            IUserRepository UserRepository,
            IMerchantRepository MerchantRepository,
            IRoleRepository RoleRepository,
            IUserRoleRepository UserRoleRepository
)
        {
            this.UserRepository = UserRepository;
            this.MerchantRepository = MerchantRepository;
            this.RoleRepository = RoleRepository;
            this.UserRoleRepository = UserRoleRepository;
            this.DbContext = DbContext;
          
        }

        public async Task<int> CommitAsync()
        {
            SetDefaultValues();
            return await DbContext.SaveChangesAsync();
        }
        private void SetDefaultValues()
        {
            foreach (var Entry in DbContext.ChangeTracker.Entries())
            {
                string ActionDate = string.Empty;
                string ActionBy = string.Empty;
                if (Entry.State == EntityState.Added)
                {
                    Dictionary<string, object> CreateDefaultValues = new();
                    ActionBy = "CreatedBy";
                    ActionDate = "CreateDate";

                    if (Entry.CurrentValues.Properties.HasProperty("IsDeleted"))
                        CreateDefaultValues.Add("IsDeleted", false);
                    if (CreateDefaultValues.Count > 0)
                        Entry.CurrentValues.SetValues(CreateDefaultValues);
                }
                else if (Entry.State == EntityState.Modified)
                {
                    if (Entry.CurrentValues.Properties.HasProperty("IsDeleted") && !Entry.OriginalValues.GetValue<bool>("IsDeleted") && Entry.CurrentValues.GetValue<bool>("IsDeleted"))
                    {
                        ActionBy = "Deletedby";
                        ActionDate = "DeleteDate";
                    }
                    else
                    {
                        ActionBy = "ModifiedBy";
                        ActionDate = "ModifiedDate";
                    }
                }
                //if (_identityProvider.Login != null && (!string.IsNullOrEmpty(ActionDate) || !string.IsNullOrEmpty(ActionBy)))
                //{
                //    Dictionary<string, object> DefaultValues = new();
                //    if (Entry.CurrentValues.Properties.HasProperty(ActionBy))
                //        DefaultValues.Add(ActionBy, _identityProvider.Login.Id);
                //    if (Entry.CurrentValues.Properties.HasProperty(ActionDate))
                //        DefaultValues.Add(ActionDate, DateTime.Now);

                //    if (DefaultValues.Count > 0)
                //        Entry.CurrentValues.SetValues(DefaultValues);
                //}

            }
        }
        //public async Task<GResponse<T>> TryCommitAsync<T>(T DbRecord)
        //{
        //    try
        //    {
        //        SetDefaultValues();   
        //        int AffectedRow = await DbContext.SaveChangesAsync();
        //        return new GResponse<T>() {
        //            Data = DbRecord,
        //            IsSucceeded = true
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.InnerException != null)
        //        {
        //            var UniqueRegex = new Regex(@"^Cannot insert duplicate key row in object '[a-zA-Z\.]+' with unique index '[a-zA-Z_]+'");
        //            var Match = UniqueRegex.Match(ex.InnerException.Message);
        //            if (Match.Success)
        //            {
        //                return new GResponse<T>()
        //                {
        //                    IsSucceeded = true,
        //                    ErrorCode = $"{new Regex("'[a-zA-Z.]+'").Match(Match.Value.Split("index")[0]).Value.Replace("'", string.Empty)}.{Match.Value.Split("index")[1].Replace("'", string.Empty).Trim()}"
        //                };
        //            }
        //        }
        //        throw;
        //    }
        //}        
    }
}
