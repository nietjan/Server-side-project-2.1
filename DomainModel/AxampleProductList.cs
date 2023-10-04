using DomainModel.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel {
    public class ExampleProductList {
        public int id {  get; set; }
        public ICollection<Product> list { get; set; } = new List<Product>();
        public TypeOfMeal type { get; set; }
    }
}
