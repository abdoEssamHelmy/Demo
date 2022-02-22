using Models.Data;
using Models.View;
using System;
using System.Linq;
namespace APIs.AutoMapperConfig
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            #region User
            CreateMap<User, CurrentLogin>()
                 .ForMember(des => des.Roles, opt => opt.MapFrom(s => s.UserRoles.Select(r=>r.Role.Name)));
            #endregion

        }
    }
}
