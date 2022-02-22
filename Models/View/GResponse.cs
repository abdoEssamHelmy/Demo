using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.View
{
    public class GResponse<T>
    {
      
        public string Status { get; set; }

        public string StatusMessage { get; set; }

        public T Data { get; set; }

        public static GResponse<T> Create(T Data,string Status,string StatusMessage = null)
        {
            return new GResponse<T>()
            {
                Data = Data,
                Status = Status,
                StatusMessage = StatusMessage
            };
        }
        public static GResponse<T> CreateFailure(string Status, string StatusMessage = null)
        {
            return new GResponse<T>()
            {
                Status = Status,
                StatusMessage = StatusMessage
            };
        }
    }
}
