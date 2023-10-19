using ApplicationServices;
using DomainModel;
using DomainModel.enums;
using DomainServices;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure {
    public class SqlRepository : IRepository {

        private PacketContext context { get; set; }

        private readonly IUserSession userSession;

        public SqlRepository(PacketContext _context, ApplicationServices.IUserSession userSession) {
            this.context = _context;
            this.userSession = userSession;
        }

        public async Task<bool> AddPacket(Packet packet) {
            context.packets.Add(packet);
            var result = await context.SaveChangesAsync();
            if (result != 1) return false;
            return true; 
        }
        
        public IEnumerable<Canteen> GetCanteens(string staffSecurityId) {
            var userCanteenList = context.canteenStaffMembers.Include(i => i.canteen).Where(i => i.securityId == staffSecurityId);
            //If user does not have canteen, return all canteens;
            if(userCanteenList.Count() != 1) {
                return context.canteen.OrderBy(i => i.city).ThenBy(i => i.location);
            }

            //if user does have canteen, put connected canteen first in list
            var list = new List<Canteen>() {
                userCanteenList.First().canteen,
            };
            var otherCanteens = context.canteen.Where(i => !i.id.Equals(userCanteenList.First().id)).OrderBy(i => i.city).ThenBy(i => i.location);
            return list.Concat(otherCanteens);
        }

        public ExampleProductList? GetExampleProducts(TypeOfMeal? typeOfMeal) {
            if (typeOfMeal == null) return null;

            //get products based of type of meal
            var list = context.exampleProductLists.Where(i => i.type == typeOfMeal);

            if (list.Count() == 0) return null;

            return list.First();
        }

        public IEnumerable<Packet> GetPackets(City? city = null, TypeOfMeal? typeOfMeal = null) {
            //return packets that are not reserved
            var list =  context.packets.Include(i => i.canteen).Include(i => i.reservedBy).Where(i => i.reservedBy == null);

            //city filer
            if (city != null) {
                list = list.Where(i => i.city == city);
            } else {
                var student = GetStudent(userSession.GetUserIdentityId());
                if(student != null) {
                    list = list.Where(i => i.city == student.studyCity);
                }
            }

            //type of meal filter
            if (typeOfMeal != null) {
                list = list.Where(i => i.typeOfMeal == typeOfMeal);
            }

            return list;
        }

        public IEnumerable<Packet>? GetPacketsOfCanteen(int id) {
            //get all packets that are connected to a canteen
            var list = context.canteen.Include(i => i.packetList).ThenInclude(i => i.reservedBy)
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
            return context.packets.Include(i => i.canteen).Where(i => i.reservedBy != null).Where(i => i.reservedBy.securityId == studentSecurityId);
        }

        public Packet? GetSinglePacket(int id) {
            var list = context.packets.Include(i => i.reservedBy).Include(i => i.canteen).Include(i => i.exampleProductList).ThenInclude(i => i.list).Where(i => i.id == id);

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

            if(context.packets.Include(i => i.reservedBy).Where(i => i.reservedBy != null)
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
            var list = context.packets.Include(i => i.reservedBy).Where(i => i.id == packetId);
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
            //check if packet exists
            var actualPacketList = context.packets.Include(i => i.canteen).Include(i => i.reservedBy).Where(i => i.id == packet.id);
            if (actualPacketList.Count() != 1) {
                return false;
            }
            var actualPacket = actualPacketList.Single();

            //change values
            actualPacket.name = packet.name;
            actualPacket.startPickup = packet.startPickup;
            actualPacket.endPickup = packet.endPickup;
            actualPacket.typeOfMeal = packet.typeOfMeal;
            actualPacket.price = packet.price;
            actualPacket.exampleProductList = packet.exampleProductList;
            actualPacket.eighteenUp = packet.eighteenUp;

            //update in DB
            context.Update(actualPacket);
            await context.SaveChangesAsync();
            return true;
        }

        public Canteen? GetCanteen(string staffSecurityId) {
            var userCanteenList = context.canteenStaffMembers.Include(i => i.canteen).Where(i => i.securityId == staffSecurityId && i.canteen != null);
            //If user does not have canteen, return all canteens;
            if (userCanteenList.Count() != 1) {
                return null;
            }
            return userCanteenList.First().canteen;
        }

        public bool UserIsCanteenStaff(string securityId) {
            var userList = context.canteenStaffMembers.Where(i => i.securityId == securityId);
            
            if(userList.Count() != 1) return false;
            else return true;
        }

        public async Task<string>? UnreservePacket(int packetId, string studentSecurityId) {
            var list = context.packets.Include(i => i.reservedBy).Where(i => i.id == packetId);

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

        //GraphQl
        public IEnumerable<Packet> GetAllPackets() {
            return context.packets;
        }

        public IEnumerable<Product> GetAllProducts() {
            return context.products;
        }
    }
}
