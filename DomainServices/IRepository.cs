using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.enums;

namespace DomainServices {
    public interface IRepository {
        //get all non reserved packets
        public IEnumerable<Packet> GetPackets(City? city = null,TypeOfMeal? typeOfMeal = null);

        //Return packets whith correct id, with non existing id returns null, contents should be orderd on date
        public IEnumerable<Packet>? GetPacketsOfCantine(int id);
        public IEnumerable<Packet> GetReservedPackets(string studentSecurityId);

        //Returns all canteens, will always return own canteen first
        public IEnumerable<Cantine> GetCantines(string staffSecurityId);

        public Cantine? GetCantine(string staffSecurityId);
        public Packet? GetSinglePacket(int id);
        public Task<bool> AddPacket(Packet packet);
        public Task<bool> UpdatePacket(Packet packet);
        public ExampleProductList? GetExampleProducts(TypeOfMeal? typeOfMeal);
        public Task<string>? ReservePacket(int packetId, string studentSecurityId);
        public Task<string>? UnreservePacket(int packetId, string studentSecurityId);
        public bool HasReservedForSpecificDay(DateTime? date, string studentSecurityId);
        public bool UserIsCanteenStaff(string securityId);
        public Student? GetStudent(string securityId);

        //GraphQl
        public IEnumerable<Packet> GetAllPackets();
        public IEnumerable<Product> GetAllProducts();
    }
}
