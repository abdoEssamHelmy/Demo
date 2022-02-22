using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.AppConfig
{
    public class AppConfig
    {
        public DBConfig DBConfig { get; set; }
        public SecurityConfig SecurityConfig { get; set; }
    }
}
