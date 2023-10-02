using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.enums;

namespace DomainModel
{
    public class Packet {
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public required string name { get; set; }

        [Required(ErrorMessage = "Please choose a city")]
        public City? city { get; set; }

        //Date when pickup starts
        [Required(ErrorMessage = "Please choose a start date")]
        public DateTime? startPickup { get; set; }


        //Date when pickup ends
        [Required(ErrorMessage = "Please choose a end date")]
        public DateTime? endPickup { get; set;}

        //Or string or Id depends on identity
        public string? reservedBy { get; set; }

        [Required(ErrorMessage = "Please choose the type of meal")]
        public TypeOfMeal? typeOfMeal { get; set; }

        [Required(ErrorMessage = "Please choose a price")]
        [Range(1, 100, ErrorMessage = "Price must be between €1 and €100")]
        [DataType(DataType.Currency)]
        public decimal? price { get; set; }

        //18+
        public bool eighteenUp { get; set; } = false;

        public Cantine? cantine { get; set; }

        //axemple products based on old products
        public ICollection<Product> axampleProducts { get; set; } = new List<Product>();

        //Actual products
        public ICollection<Product> products { get; set; } = new List<Product>();
    }
}
