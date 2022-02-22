
namespace Models.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
       public User()
        {
            UserRoles=new HashSet<UserRole>();
        }
        public int Id { get; set; }
        public int MerchantId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

    }
}
