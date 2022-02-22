using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.AppConfig
{
    public class HttpLoggingModes
    {
        public const string None = "None";
        public const string Basic = "Basic";        
        public const string Full = "Full";
    }
    public class HttpLogging
    {
        public string Mode { get; set; }
    }
}
