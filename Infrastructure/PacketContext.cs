using DomainModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure {
    public class PacketContext : DbContext, IDisposable {
        public PacketContext(DbContextOptions<PacketContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder builder) {

            //add primary keys
            builder.Entity<Packet>().HasKey(i => i.id);
            builder.Entity<Packet>().HasKey(i => i.id);
            builder.Entity<Canteen>().HasKey(i => i.id);
            builder.Entity<CanteenStaffMember>().HasKey(i => i.id);
            builder.Entity<ExampleProductList>().HasKey(i => i.id);
        }


        public DbSet<Product> products { get; set; }
        public DbSet<Packet> packets { get; set; }
        public DbSet<Canteen> canteen { get; set; }
        public DbSet<CanteenStaffMember> canteenStaffMembers { get; set; }
        public DbSet<ExampleProductList> exampleProductLists { get; set; }
        public DbSet<Student> students { get; set; }
    }
}
