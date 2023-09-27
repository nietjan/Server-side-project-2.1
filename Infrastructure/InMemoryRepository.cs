using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.enums;
using DomainServices;

namespace Infrastructure {
    public class InMemoryRepository : IRepository {
        //products for dummy packets list
        private static readonly Cantine cantine = new Cantine() {id = 1, city = City.Breda, location = "Hogenschoollaan", servesHotMeals = true};

        private static List<Packet> packets = new List<Packet>() {
            new Packet() {
                id = 1,
                name = "Packet1",
                cantine = cantine,
                city = cantine.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Diner,
                price = 8,
                eighteenUp = true,
            },
            new Packet() {
                id = 2,
                name = "Packet2",
                cantine = cantine,
                city = cantine.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Drink,
                price = 5,
                eighteenUp = true
            },new Packet() {
                id = 3,
                name = "Packet3",
                cantine = cantine,
                city = cantine.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Bread,
                price = 10,
                eighteenUp = true
            },
        };

        public InMemoryRepository() {
            cantine.packetList = packets;
        }

        
        public async Task<bool> addPacket(Packet packet) {
            packets.Add(packet);
            return true;
        }

        public IEnumerable<Product> GetAxampleProducts(int id) {
            return packets.ElementAt(id).axampleProducts;
        }

        public IEnumerable<Packet> GetPackets() {
            return packets;
        }

        public IEnumerable<Packet> GetSinglePacket(int id) {
            return packets.Where(i => i.id == id);
        }

        public async Task<bool> reservePacket(int packetId, string personEmail) {
            packets.ElementAt(packetId).reservedBy = personEmail;
            return true;
        }
    }
}
