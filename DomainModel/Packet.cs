using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.enums;

namespace DomainModel
{
    public class Packet {
        public int id { get; set; }

        public required string name { get; set; }

        public City city { get; set; }

        //Date when pickup starts
        public DateTime startPickup { get; set; }


        //Date when pickup ends
        public DateTime endPickup { get; set;}

        //Or string or Id depends on identity
        public string? reservedBy { get; set; } 

        public TypeOfMeal typeOfMeal { get; set; }

        public int price { get; set; }

        //18+
        public bool eighteenUp { get; set; }

        public required Cantine cantine { get; set; }

        //axemple products based on old products
        public ICollection<Product> axampleProducts { get; set; } = new List<Product>();

        //Actual products
        public ICollection<Product> products { get; set; } = new List<Product>();
    }
}
