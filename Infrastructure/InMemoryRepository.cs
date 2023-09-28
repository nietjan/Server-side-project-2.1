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
        public static readonly Cantine cantine = new Cantine() {id = 1, city = City.Breda, location = "Hogenschoollaan", servesHotMeals = true};

        public List<Packet> packets { get; set; } = new List<Packet>() {
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
                eighteenUp = true,
            },new Packet() {
                id = 3,
                name = "Packet3",
                cantine = cantine,
                city = cantine.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Bread,
                price = 10,
                eighteenUp = true,
                reservedBy = "test@test.com"
            },
        };

        public InMemoryRepository() {
            cantine.packetList = packets;
        }

        
        public async Task<bool> AddPacket(Packet packet) {
            //add id
            if (packet.id == 0) {
                packet.id = packets.ElementAt(packets.Count() - 1).id+1;
            }

            packets.Add(packet);
            return true;
        }

        public IEnumerable<Product>? GetAxampleProducts(int id) {
            if(id <= 0 || id > packets.Count()-1) {
                return null;
            }

            return packets.Where(i => i.id == id).First().axampleProducts;
        }

        public IEnumerable<Packet> GetPackets() {
            return packets.Where(i => i.reservedBy == null);
        }

        public IEnumerable<Packet> GetReservedPackets(string email) {
            return packets.Where(i => i.reservedBy == email);
        }

        public Packet? GetSinglePacket(int id) {
            var list = packets.Where(i => i.id == id);

            if(list.Count() > 0) {
                return list.First();
            } else {
                return null;
            }
        }

        public async Task<bool> reservePacket(int packetId, string personEmail) {
            var list = packets.Where(i => i.id == packetId);

            if (list.Count() == 0) {
                return false;
            }

            packets.ElementAt(packetId).reservedBy = personEmail;
            return true;
        }
    }
}
