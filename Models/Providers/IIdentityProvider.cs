using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Providers
{
    public interface IIdentityProvider
    {
        public CurrentLogin Login { get; }
    }
}
