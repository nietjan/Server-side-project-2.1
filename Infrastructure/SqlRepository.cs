using DomainModel;
using DomainModel.enums;
using DomainServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure {
    public class SqlRepository : IRepository {

        private PacketContext context { get; set; }
        public SqlRepository(PacketContext _context) {
            this.context = _context;
        }

        public async Task<bool> AddPacket(Packet packet) {
            context.packets.Add(packet);
            var result = await context.SaveChangesAsync();
            if (result != 1) return false;
            return true; 
        }

        public IEnumerable<Cantine> GetCantines(int userId) {
            var canteenUser = context.canteenStaffMembers.Where(i => i.id == userId).First();
            //If user does not have canteen, return all canteens;
            if(canteenUser == null) {
                return context.canteen;
            }

            //if user does have canteen, put connected canteen first in list
            var list = new List<Cantine>() {
                canteenUser.cantine,
            };
            return list.Concat(context.canteen.Where(i => i.id != canteenUser.id));
        }

        public ExampleProductList? GetExampleProducts(TypeOfMeal? typeOfMeal) {
            if (typeOfMeal == null) return null;

            //get products based of type of meal
            var list = context.exampleProductLists.Where(i => i.type == typeOfMeal);

            if (list.Count() == 0) return null;

            return list.First();
        }

        public IEnumerable<Packet> GetPackets() {
            //return packets that are not reserved
            return context.packets.Where(i => i.reservedBy == null);
        }

        public IEnumerable<Packet>? GetPacketsOfCantine(int id) {
            //get all packets that are connected to a canteen
            var list = context.canteen
                .Where(i => i.id == id);
                
            //if not exactly 1 item, return null
            if(list.Count() != 1) {
                return null;
            } 
            
            return list
                .Single()
                .packetList
                .OrderBy(i => i.startPickup);
        }

        public IEnumerable<Packet> GetReservedPackets(string email) {
            //returns all packets that are reserved by a specific email
            return context.packets.Where(i => i.reservedBy == email);
        }

        public Packet? GetSinglePacket(int id) {
            var list = context.packets.Where(i => i.id == id);

            if (list.Count() > 0) {
                return list.First();
            } else {
                return null;
            }
        }

        public bool hasReservedForSpecificDay(DateTime? packetDate, string personEmail) {
            if (packetDate == null) {
                return false;
            }

            if (context.packets.Where(i => i.reservedBy == personEmail
            && i.startPickup.Value.Day == packetDate.Value.Day
            && i.startPickup.Value.Month == packetDate.Value.Month
            && i.startPickup.Value.Year == packetDate.Value.Year)
                .Count() != 0) {
                return true;
            }
            return false;
        }

        public async Task<string>? reservePacket(int packetId, string personEmail) {
            var list = context.packets.Where(i => i.id == packetId);

            if (list.Count() == 0) {
                return "Packet not found";
            }

            var packet = list.First();
            if (packet.reservedBy != null) {
                return "Packet already reserved";
            }

            //check if user already reserved a package for that day
            if (context.packets.Where(i => i.reservedBy == personEmail
            && i.startPickup.Value.Day == packet.startPickup.Value.Day
            && i.startPickup.Value.Month == packet.startPickup.Value.Month
            && i.startPickup.Value.Year == packet.startPickup.Value.Year)
                .Count() != 0) {
                return "Already reserved a package";
            }


            packet.reservedBy = personEmail;
            await context.SaveChangesAsync();
            return null;
        }
    }
}
