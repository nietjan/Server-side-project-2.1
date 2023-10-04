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
            //insert data
        }

        public DbSet<Product> products { get; set; }
        public DbSet<Packet> packets { get; set; }
        public DbSet<Cantine> canteen { get; set; }
        public DbSet<CantineStaffMember> canteenStaffMembers { get; set; }
        public DbSet<ExampleProductList> exampleProductLists { get; set; }
    }
}
