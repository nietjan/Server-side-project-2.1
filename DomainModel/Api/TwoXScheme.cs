using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Api {
    public class TwoXScheme {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; }
    }
}
