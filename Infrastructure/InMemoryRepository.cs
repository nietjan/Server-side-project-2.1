using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Net.Sockets;
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
                startPickup = DateTime.Now.AddDays(1),
                endPickup = DateTime.Now.AddDays(2),
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
                eighteenUp = false,
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

        public async Task<string>? reservePacket(int packetId, string personEmail) {
            var list = packets.Where(i => i.id == packetId);

            if (list.Count() == 0) {
                return "Packet not found";
            }

            var packet = list.First();
            if(packet.reservedBy != null) {
                return "Packet already reserved";
            }

            //check if user already reserved a package for that day
            if (packets.Where(i => i.reservedBy == personEmail 
            && i.startPickup.Value.Day == packet.startPickup.Value.Day 
            && i.startPickup.Value.Month == packet.startPickup.Value.Month
            && i.startPickup.Value.Year == packet.startPickup.Value.Year)
                .Count() != 0) {
                return "Already reserved a package";
            }


            packet.reservedBy = personEmail;
            return null;
        }

        public IEnumerable<Packet>? GetPacketsOfCantine(int id) {
            if(id != cantine.id) {
                return null;
            }

            return packets.Where(i => i.cantine.id == id).OrderBy(i => i.startPickup);
        }

        public IEnumerable<Cantine> GetCantines(int userId) {
            //In real Repo should return all cantines of table cantine and returns userId Canteen first
            return new List<Cantine>() { cantine, cantine, cantine};
        }

        public bool hasReservedForSpecificDay(DateTime? packetDate) {
            if(packetDate == null) {
                return false;
            }

            string personEmail = "test@test.com";
            if (packets.Where(i => i.reservedBy == personEmail
            && i.startPickup.Value.Day == packetDate.Value.Day
            && i.startPickup.Value.Month == packetDate.Value.Month
            && i.startPickup.Value.Year == packetDate.Value.Year)
                .Count() != 0) {
                return true;
            }
            return false;
        }
    }
}
