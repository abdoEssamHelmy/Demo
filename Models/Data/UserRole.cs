
namespace Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class UserRole
    {
        public int Id { get; set; }
        public int MerchantId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
