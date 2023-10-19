using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DomainModel.overload;
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

        public Student? reservedBy { get; set; }

        [Required(ErrorMessage = "Please choose the type of meal")]
        public TypeOfMeal? typeOfMeal { get; set; }

        [Required(ErrorMessage = "Please choose a price")]
        [Range(1, 100, ErrorMessage = "Price must be between €1 and €100")]
        [DataType(DataType.Currency)]
        public decimal? price { get; set; }

        //18+
        public bool eighteenUp { get; set; } = false;

        public Canteen? canteen { get; set; }

        //example products based on old products
        public ExampleProductList? exampleProductList { get; set; }

        public void SetEighteenUpValue() {
            //check if exampleProductlist exists
            if(exampleProductList == null) {
                eighteenUp = false;
                return;
            }

            //loop over product list and check if a product is 18+
            foreach(var product in exampleProductList.list) {
                if (product == null) continue;

                if (product.alcoholic) {
                    eighteenUp = true;
                    return;
                }
            }

            //no products are 18+
            eighteenUp = false;
        }

        public bool StudentIsAllowedToReservePacketByAge(Student? student) {
            if(student == null) return false;

            // if 18 always allowed
            if (student.birthday.getAge() >= 18) {
                return true;
            }

            // if packet is 18+ than not allowed, because student is no 18+
            if (eighteenUp == true) {
                return false;
            }

            // student is not 18 and packet is not 18+ so allowed
            return true;
        }
    }
}
