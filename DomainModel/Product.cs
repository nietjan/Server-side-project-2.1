using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel {
    public class Product {
        public int id { get; set; }

        public required string name { get; set; }
        
        public bool alcoholic { get; set; }

        public string? imageUrl { get; set; }
    }
}
