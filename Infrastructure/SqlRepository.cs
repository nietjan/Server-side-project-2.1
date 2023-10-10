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
        
        public IEnumerable<Cantine> GetCantines(string staffSecurityId) {
            var userCanteenList = context.canteenStaffMembers.Where(i => i.securityId == staffSecurityId);
            //If user does not have canteen, return all canteens;
            if(userCanteenList.Count() != 1) {
                return context.canteen;
            }

            //if user does have canteen, put connected canteen first in list
            var list = new List<Cantine>() {
                userCanteenList.First().cantine,
            };
            return list.Concat(context.canteen.Where(i => !i.id.Equals(userCanteenList.First().id)));
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

        public IEnumerable<Packet> GetReservedPackets(string studentSecurityId) {
            //returns all packets that are reserved by a specific email
            return context.packets.Where(i => i.reservedBy != null).Where(i => i.reservedBy.securityId == studentSecurityId);
        }

        public Packet? GetSinglePacket(int id) {
            var list = context.packets.Where(i => i.id == id);

            if (list.Count() > 0) {
                return list.First();
            } else {
                return null;
            }
        }

        public bool HasReservedForSpecificDay(DateTime? packetDate, string studentSecurityId) {
            if (packetDate == null) {
                return false;
            }

            if(context.packets.Where(i => i.reservedBy != null)
                .Where(i => i.reservedBy.securityId == studentSecurityId
            && i.startPickup.Value.Day == packetDate.Value.Day
            && i.startPickup.Value.Month == packetDate.Value.Month
            && i.startPickup.Value.Year == packetDate.Value.Year)
                .Count() != 0) {
                return true;
            }
            return false;
        }

        public async Task<string>? ReservePacket(int packetId, string studentSecurityId) {
            //check if package exists
            var list = context.packets.Where(i => i.id == packetId);
            if (list.Count() == 0) {
                return "Packet not found";
            }

            //check if package is reserved
            var packet = list.First();
            if (packet.reservedBy != null) {
                return "Packet already reserved";
            }

            //check if user already reserved a package for that day
            if (context.packets.Where(i => i.reservedBy != null)
                .Where(i => i.reservedBy.securityId == studentSecurityId
            && i.startPickup.Value.Day == packet.startPickup.Value.Day
            && i.startPickup.Value.Month == packet.startPickup.Value.Month
            && i.startPickup.Value.Year == packet.startPickup.Value.Year)
                .Count() != 0) {
                return "Already reserved a package";
            }

            //check if student exists
            var student = GetStudent(studentSecurityId);
            if(student == null) { 
                return "Student cannot be found";
            }

            //check if student is old enough
            if (packet.StudentIsAllowedToReservePacketByAge(student) == false) {
                return "Student not old enough to reserve packet";
            }

            //reserve packet
            packet.reservedBy = student;
            await context.SaveChangesAsync();
            return null;
        }

        public async Task<bool> UpdatePacket(Packet packet) {
            if(context.packets.Where(i => i.id == packet.id).Count() != 1) {
                return false;
            }

            context.Update(packet);
            await context.SaveChangesAsync();
            return true;
        }

        public Cantine? GetCantine(string staffSecurityId) {
            var userCanteenList = context.canteenStaffMembers.Where(i => i.securityId == staffSecurityId && i.cantine != null);
            //If user does not have canteen, return all canteens;
            if (userCanteenList.Count() != 1) {
                return null;
            }
            return userCanteenList.First().cantine;
        }

        public bool UserIsCanteenStaff(string securityId) {
            var userList = context.canteenStaffMembers.Where(i => i.securityId == securityId);
            if(userList.Count() != 1) {
                return false;
            }
            return true;
        }

        public async Task<string>? UnreservePacket(int packetId, string studentSecurityId) {
            var list = context.packets.Where(i => i.id == packetId);

            if (list.Count() == 0) {
                return "Packet not found";
            }

            var packet = list.First();
            if (packet.reservedBy == null) {
                return "Packet was not reserved";
            }

            //check if package is reserved by user
            if (packet.reservedBy.securityId != studentSecurityId) {
                return "Packet is not reserved by user";
            }

            packet.reservedBy = null;
            await context.SaveChangesAsync();
            return null;
        }

        public Student? GetStudent(string securityId) {
            var list = context.students.Where(i => i.securityId == securityId);

            if (list.Count() != 1) return null;
            return list.Single();
        }
    }
}
