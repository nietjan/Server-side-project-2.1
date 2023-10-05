using DomainModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure {
    public class PacketContext : DbContext {
        public PacketContext(DbContextOptions<PacketContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder builder) {

            //add primary keys
            builder.Entity<Packet>().HasKey(i => i.id);
            builder.Entity<Packet>().HasKey(i => i.id);
            builder.Entity<Cantine>().HasKey(i => i.id);
            builder.Entity<CantineStaffMember>().HasKey(i => i.id);
            builder.Entity<ExampleProductList>().HasKey(i => i.id);

            //add start info
            //Cantine[] canteens = { 
            //    new Cantine() {id=1, location = "Hogeschoollaan", city = DomainModel.enums.City.Breda, servesHotMeals = true }, 
            //    new Cantine() {id=2, location = "FifthLa", city = DomainModel.enums.City.Breda, servesHotMeals = true }, 
            //    new Cantine() {id=3, location = "ld", city = DomainModel.enums.City.Breda, servesHotMeals = false } };
            
            //CantineStaffMember[] staff = { 
            //    new CantineStaffMember() {id=1, cantine = canteens.ElementAt(0), name = "Staff1-Hogeschoolaan", staffNumber = 123 },
            //    new CantineStaffMember() {id = 2,  cantine = canteens.ElementAt(1), name = "Staff2-FifhtLa", staffNumber = 234 },
            //    new CantineStaffMember() {id=3, cantine = canteens.ElementAt(2), name = "Staff3-Ld", staffNumber = 345 }};

            //Product[] productsList = { 
            //    new Product() {id=1, alcoholic=true, name="bread1" },
            //    new Product() {id=2,alcoholic=true, name="bread2" },
            //    new Product() {id=3,alcoholic=false, name="bread3"},

            //    new Product() {id=4,alcoholic=true, name="diner1"},
            //    new Product() {id=5,alcoholic=false, name="diner2"},
            //    new Product() {id=6, alcoholic=false, name="diner3"},

            //    new Product() {id=7,alcoholic=false, name="drink1"},
            //    new Product() {id=8,alcoholic=false, name="drink2"},
            //    new Product() {id=9,alcoholic=false, name="drink3"},
            //};



            //ExampleProductList[] exampleList = {
            //    new ExampleProductList() {id=1,list = new List<Product>(){ productsList[0], productsList[1], productsList[2]}, type = DomainModel.enums.TypeOfMeal.Bread},
            //    new ExampleProductList() {id=2,list = new List<Product>(){ productsList[3], productsList[4], productsList[5]}, type = DomainModel.enums.TypeOfMeal.Diner},
            //    new ExampleProductList() {id = 3, list = new List<Product>(){ productsList[6], productsList[7], productsList[8]}, type = DomainModel.enums.TypeOfMeal.Drink},
            //};

            //builder.Entity<Cantine>().HasData(canteens);
            //builder.Entity<CantineStaffMember>().HasData(staff); 

            //builder.Entity<ExampleProductList>().HasData(exampleList);
            //builder.Entity<ExampleProductList>().OwnsMany(i => i.list).HasData(productsList);
            //builder.Entity<CantineStaffMember>().HasData(staff);
        }

        public DbSet<Product> products { get; set; }
        public DbSet<Packet> packets { get; set; }
        public DbSet<Cantine> canteen { get; set; }
        public DbSet<CantineStaffMember> canteenStaffMembers { get; set; }
        public DbSet<ExampleProductList> exampleProductLists { get; set; }
    }
}
