using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.enums;

namespace DomainServices {
    public interface IRepository {
        public IEnumerable<Packet> GetPackets();

        //Return packets whith correct id, with non existing id returns null, contents should be orderd on date
        public IEnumerable<Packet>? GetPacketsOfCantine(int id);
        public IEnumerable<Packet> GetReservedPackets(string email);

        //Returns all canteens, will always return own canteen first
        public IEnumerable<Cantine> GetCantines(int userId);

        public Packet? GetSinglePacket(int id);
        public Task<bool> AddPacket(Packet packet);
        public ExampleProductList? GetExampleProducts(TypeOfMeal? typeOfMeal);
        public Task<string>? reservePacket(int packetId, string personEmail);
        public bool hasReservedForSpecificDay(DateTime? date, string personEmail);
    }
}
