using DomainModel;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class SeedData {
        private readonly PacketContext _context;
        private readonly SecurityContext _securityContext;

        public SeedData(PacketContext dbContext, SecurityContext securityContext) {
            _context = dbContext;
            _securityContext = securityContext;
        }

        public void SeedDatabase() {
            _context.Database.Migrate();
            if (_context.canteen?.Count() == 0) {
                //seedDbContext();
            }

            
        }

        public void seedDbContext() {
            Cantine[] canteens = {
                    new Cantine() {id=1, location = "Hogeschoollaan", city = DomainModel.enums.City.Breda, servesHotMeals = true },
                    new Cantine() {id=2, location = "FifthLa", city = DomainModel.enums.City.Breda, servesHotMeals = true },
                    new Cantine() {id=3, location = "ld", city = DomainModel.enums.City.Breda, servesHotMeals = false } };

            CantineStaffMember[] staff = { //Add id of security
                    new CantineStaffMember() {cantine = canteens.ElementAt(0), name = "Staff1-Hogeschoolaan", staffNumber = 123 },
                    new CantineStaffMember() {cantine = canteens.ElementAt(1), name = "Staff2-FifhtLa", staffNumber = 234 },
                    new CantineStaffMember() {cantine = canteens.ElementAt(2), name = "Staff3-Ld", staffNumber = 345 }};

            Product[] productsList = {
                    new Product() {id=1, alcoholic=true, name="bread1" },
                    new Product() {id=2,alcoholic=true, name="bread2" },
                    new Product() {id=3,alcoholic=false, name="bread3"},

                    new Product() {id=4,alcoholic=true, name="diner1"},
                    new Product() {id=5,alcoholic=false, name="diner2"},
                    new Product() {id=6, alcoholic=false, name="diner3"},

                    new Product() {id=7,alcoholic=false, name="drink1"},
                    new Product() {id=8,alcoholic=false, name="drink2"},
                    new Product() {id=9,alcoholic=false, name="drink3"},
                };

            ExampleProductList[] exampleList = {
                    new ExampleProductList() {id=1,list = new List<Product>(){ productsList[0], productsList[1], productsList[2]}, type = DomainModel.enums.TypeOfMeal.Bread},
                    new ExampleProductList() {id=2,list = new List<Product>(){ productsList[3], productsList[4], productsList[5]}, type = DomainModel.enums.TypeOfMeal.Diner},
                    new ExampleProductList() {id = 3, list = new List<Product>(){ productsList[6], productsList[7], productsList[8]}, type = DomainModel.enums.TypeOfMeal.Drink},
                };

            _context.canteen.AddRange(canteens);
            _context.canteenStaffMembers.AddRange(staff);
            _context.products.AddRange(productsList);
            _context.exampleProductLists.AddRange(exampleList);
            _context.SaveChangesAsync().Wait();
        }

        public void seedSecurityContext() {

        }
    }

}
