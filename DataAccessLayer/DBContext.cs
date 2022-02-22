using Microsoft.EntityFrameworkCore;
using Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions Options) : base(Options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>().HasQueryFilter(r => r.IsActive);
            modelBuilder.Entity<UserRole>().HasQueryFilter(r => r.IsActive);
            modelBuilder.Entity<Merchant>().HasQueryFilter(r => r.IsActive);

         
        }

        public virtual DbSet<Merchant> Merchants { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
    }
}
