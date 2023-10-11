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

        public static List<ExampleProductList> productsExampleList = new List<ExampleProductList>() {
            new ExampleProductList() {
                id = 1,
                list = new List<Product>() {
                    new Product() {
                        id = 1,
                        name = "Bread1",
                        alcoholic = false,
                    }, new Product() {
                        id = 2,
                        name = "Bread2",
                        alcoholic = false,
                    }, new Product() {
                        id = 3,
                        name = "Bread3",
                        alcoholic = false,
                    }
                },
                type = TypeOfMeal.Bread
            },
            new ExampleProductList() {
                id = 2,
                list = new List<Product>() {
                    new Product() {
                        id = 4,
                        name = "Diner1",
                        alcoholic = true,
                    }, new Product() {
                        id = 5,
                        name = "Diner2",
                        alcoholic = false,
                    }, new Product() {
                        id = 6,
                        name = "Diner3",
                        alcoholic = true,
                    }
                },
                type = TypeOfMeal.Diner
            },
            new ExampleProductList() {
                id = 3,
                list = new List<Product>() {
                    new Product() {
                        id = 7,
                        name = "Drink1",
                        alcoholic = false,
                    }, new Product() {
                        id = 8,
                        name = "Drink2",
                        alcoholic = true,
                    }, new Product() {
                        id = 9,
                        name = "Drink3",
                        alcoholic = true,
                    }
                },
                type = TypeOfMeal.Drink
            },
        };

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
                exampleProductList = productsExampleList.ElementAt(1)
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
                exampleProductList = productsExampleList.ElementAt(2)
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
                reservedBy=new Student() {securityId="test@test.com", name="", studentNumber=123, studyCity = City.Breda},
                exampleProductList = productsExampleList.ElementAt(0)
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

        public IEnumerable<Packet> GetPackets() {
            return packets.Where(i => i.reservedBy == null);
        }

        public IEnumerable<Packet> GetReservedPackets(string studentSecurityId) {
            return packets.Where(i => i.reservedBy != null).Where(i => i.reservedBy.securityId == studentSecurityId);
        }

        public Packet? GetSinglePacket(int id) {
            var list = packets.Where(i => i.id == id);

            if(list.Count() > 0) {
                return list.First();
            } else {
                return null;
            }
        }

        public async Task<string>? ReservePacket(int packetId, string studentSecurityId) {
            var list = packets.Where(i => i.id == packetId);

            if (list.Count() == 0) {
                return "Packet not found";
            }

            var packet = list.First();
            if(packet.reservedBy != null) {
                return "Packet already reserved";
            }

            //check if user already reserved a package for that day
            if (packets.Where(i => i.reservedBy != null)
                .Where(i => i.reservedBy.securityId == studentSecurityId
            && i.startPickup.Value.Day == packet.startPickup.Value.Day 
            && i.startPickup.Value.Month == packet.startPickup.Value.Month
            && i.startPickup.Value.Year == packet.startPickup.Value.Year)
                .Count() != 0) {
                return "Already reserved a package";
            }

            if(packet.StudentIsAllowedToReservePacketByAge(GetStudent(studentSecurityId)) == false) {
                return "Student not old enough to reserve packet";
            }

            packet.reservedBy = new Student() { securityId=studentSecurityId, name="", studentNumber=123, studyCity=City.Breda};
            return null;
        }

        public IEnumerable<Packet>? GetPacketsOfCantine(int id) {
            if(id != cantine.id) {
                return null;
            }

            return packets.Where(i => i.cantine.id == id).OrderBy(i => i.startPickup);
        }

        public IEnumerable<Cantine> GetCantines(string staffSecurityId) {
            //In real Repo should return all cantines of table cantine and returns userId Canteen first
            return new List<Cantine>() { cantine, cantine, cantine};
        }

        public bool HasReservedForSpecificDay(DateTime? packetDate,  string studentSecurityId) {
            if(packetDate == null) {
                return false;
            }

            if (packets.Where(i => i.reservedBy != null)
                .Where(i => i.reservedBy.securityId == studentSecurityId
            && i.startPickup.Value.Day == packetDate.Value.Day
            && i.startPickup.Value.Month == packetDate.Value.Month
            && i.startPickup.Value.Year == packetDate.Value.Year)
                .Count() != 0) {
                return true;
            }
            return false;
        }

        public ExampleProductList? GetExampleProducts(TypeOfMeal? typeOfMeal) {
            if (typeOfMeal == null) return null;

            //get products based of type of meal
            var list = productsExampleList.Where(i => i.type == typeOfMeal);

            if(list.Count() == 0) return null;
            
            return list.First();
        }

        public async Task<bool> UpdatePacket(Packet packet) {
            var list = packets.Where(i => i.id == packet.id);
            if(list.Count() != 1) {
                return false;
            }

            packets.Remove(list.First());
            packets.Add(packet);
            return true;
        }

        public Cantine? GetCantine(string staffSecurityId) {
            return cantine;
        }

        public bool UserIsCanteenStaff(string securityId) {
            return false;
        }

        public async Task<string>? UnreservePacket(int packetId, string studentSecurityId) {
            var list = packets.Where(i => i.id == packetId);

            if (list.Count() == 0) {
                return "Packet not found";
            }

            var packet = list.First();
            if (packet.reservedBy == null) {
                return "Packet was not reserved";
            }

            //check if package is reserved by user
            if(packet.reservedBy.securityId != studentSecurityId) {
                return "Packet is not reserved by user";
            }

            packet.reservedBy = null;
            return null;
        }

        public Student? GetStudent(string securityId) {
            if (securityId == "99aed502-3fab-4077-acca-d74391cecd3f") return new Student() { securityId = securityId, name = "Student2", birthday = new DateTime(2004, 1, 1), studentNumber = 234, studyCity = DomainModel.enums.City.Breda };
            return new Student() { securityId = securityId, name = "Student1", birthday = new DateTime(2006, 1, 1), studentNumber = 123, studyCity = DomainModel.enums.City.Breda };
            
        }

        public IEnumerable<Packet> GetAllPackets() {
            return packets;
        }

        public IEnumerable<Product> GetAllProducts() {
            List<Product> list = new();

            foreach(var productsList in productsExampleList) {
                list.AddRange(productsList.list);
            }

            return list;
        }
    }
}
