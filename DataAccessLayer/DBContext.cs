using Microsoft.EntityFrameworkCore;
using Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Providers;

namespace DataAccessLayer
{
    public class DBContext : DbContext
    {
        private readonly IIdentityProvider _identityProvider;
        public DBContext(DbContextOptions Options, IIdentityProvider identityProvider) : base(Options)
        {
            _identityProvider = identityProvider;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>().HasQueryFilter(r => r.IsActive);
            modelBuilder.Entity<UserRole>().HasQueryFilter(r => r.IsActive);
            if(_identityProvider.Login!=null)
                modelBuilder.Entity<User>().HasQueryFilter(b => EF.Property<int>(b, "MerchantId") == _identityProvider.Login.MerchantId);
            modelBuilder.Entity<Merchant>().HasQueryFilter(r => r.IsActive);

         
        }

        public virtual DbSet<Merchant> Merchants { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
    }
}
