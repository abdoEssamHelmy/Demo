﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.View
{
    public class CurrentLogin
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int MerchantId { get; set; }
        public List<string> Roles { get; set; }
    }
}
