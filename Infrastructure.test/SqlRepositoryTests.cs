using DomainModel.enums;
using DomainModel;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Infrastructure.test {
    public class SqlRepositoryTests : IDisposable {
        private PacketContext _context { get; set; }
        private SqlRepository repository { get; set; }

        public SqlRepositoryTests() {
            var optionsBuilder = new DbContextOptionsBuilder<PacketContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "AuthorDb");
            _context = new PacketContext(optionsBuilder.Options);
            repository = new SqlRepository(_context);
    }
        public void Dispose() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async void ClearDb() {
            _context.packets.RemoveRange(_context.packets);
            _context.canteen.RemoveRange(_context.canteen);
            _context.canteenStaffMembers.RemoveRange(_context.canteenStaffMembers);
            _context.products.RemoveRange(_context.products);
            _context.exampleProductLists.RemoveRange(_context.exampleProductLists);
            
            await _context.SaveChangesAsync();
        }

        //    //Act
        //    var exception = Record.ExceptionAsync(() => repo.AddPacket(packet));

        //    //Assert
        //    Assert.NotNull(exception);
        //        Assert.Equal(packet, _context.packets.Last());

        //Add Packet
        [Fact]
        public async void Packet_Without_Id_Should_Be_Added() {
            //Arrange
            Packet packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1};
             
            //Act
            await repository.AddPacket(packet);

            //Assert
            Assert.Equal(packet, _context.packets.Last());
        }

        [Fact]
        public async void Packet_Without_Id_Gets_Id_When_Added() {
            //Arrange 
            Packet packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };

            //Act
            await repository.AddPacket(packet);

            //Assert
            Assert.NotEqual(0, _context.packets.Last().id);
        }

        [Fact]
        public async void Packet_With_Id_Keeps_Id_When_Added() {
            //Arrange
            Packet packet = new Packet() { name = "", id=4, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };

            //Act
            await repository.AddPacket(packet);

            //Assert
            Assert.Equal(4, _context.packets.Last().id);
        }

        //GetPackets
        [Fact]
        public async void Get_Only_Packets_Whom_Are_Not_Reserved() {
            //Arrange
            Packet packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet2 = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet3 = new Packet() { name = "", reservedBy="", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            
            _context.Add(packet);
            _context.Add(packet2);
            _context.Add(packet3);
            await _context.SaveChangesAsync();

            //Act
            var products = repository.GetPackets();

            //Assert
            Assert.DoesNotContain(packet3, products);
        }

        //GetPacketsOfCanteen
        [Fact]
        public async void Get_All_Packets_Of_Canteen_With_Valid_Id() {
            //Arrange
            _context.canteen.Add(new Cantine() {
                location = "",
                city = City.Breda,
                id = 1,
                servesHotMeals = false,
                packetList = new List<Packet>() {
                    new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },
                    new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },
                    new Packet() { name = "", reservedBy = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },

                }
            });
            await _context.SaveChangesAsync();   

            //Act
            var packets = repository.GetPacketsOfCantine(1);

            //Assert
            Assert.NotNull(packets);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async void Get_All_Packets_Of_Canteen_With_Invalid_Id_Returns_Null(int id) {
            //Arrange
            _context.canteen.Add(new Cantine() {
                location = "",
                city = City.Breda,
                id = 1,
                servesHotMeals = false,
                packetList = new List<Packet>() {
                    new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },
                    new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },
                    new Packet() { name = "", reservedBy = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },

                }
            });
            await _context.SaveChangesAsync();


            //Act
            var packets = repository.GetPacketsOfCantine(id);

            //Assert
            Assert.Null(packets);
        }

        //GetReservedPackets
        [Fact]
        public async void Get_Packets_Whom_Are_Reserved_With_Correct_Email() {
            //Arrange
            var packet = new Packet() { name = "", reservedBy = "test@test.com", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var products = repository.GetReservedPackets("test@test.com");

            //Assert
            Assert.Contains(packet, _context.packets);
        }

        [Fact]
        public async void Get_List_Of_Packets_Whom_Are_Reserved_With_InCorrect_Email_That_Is_Empty() {
            //Arrange
            SqlRepository repository = new SqlRepository(_context);
            
            _context.packets.Add(new Packet() { name = "", reservedBy = "test@test.com", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 });
            await _context.SaveChangesAsync();

            //Act
            var products = repository.GetReservedPackets("test2@test2.nl");

            //Assert
            Assert.Empty(products);
        }

        //GetExampleProducts
        [Fact]
        public async void Get_Single_Product_With_Correct_Id() {
            //Arrange
            var packet = new Packet() { name = "", id=1, reservedBy = "test@test.com", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var methodPacket = repository.GetSinglePacket(1);

            //Assert
            Assert.NotNull(methodPacket);
            Assert.Equal(packet, methodPacket);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public async void Get_Single_Product_With_Incorrect_Id_That_Is_Null(int id) {
            //Arrange
            var packet = new Packet() { name = "", reservedBy = "test@test.com", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var methodPacket = repository.GetSinglePacket(id);

            //Assert
            Assert.Null(methodPacket);
            Assert.NotEqual(packet, methodPacket);
        }

        //Reserve packet
        [Fact]
        public async void Reserve_Packet_With_Correct_Info_Should_Return_Null() {
            //Arrange
            var packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.reservePacket(packet.id, "test@test.nl");

            //Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public async void Reserve_Packet_With_Id_Lower_Or_Equal_To_0_Should_Return_Package_Not_Found(int id) {
            //Arrange
            var packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.reservePacket(id, "test@test.nl");

            //Assert
            Assert.Equal("Packet not found", result);
        }

        [Theory]
        [InlineData(400)]
        [InlineData(100)]
        public async void Reserve_Packet_With_Not_Existing_Id_Should_Return_Package_Not_Found(int id) {
            //Arrange
            var packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.reservePacket(id, "test@test.nl");

            //Assert
            Assert.Equal("Packet not found", result);
        }

        [Fact]
        public async void Reserve_Packet_With_Id_Of_Already_Reserved_Package_Should_Return_Package_Already_Reserved() {
            //Arrange
            var packet = new Packet() { name = "", id=1, reservedBy = "test@test.com", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.reservePacket(1, "test@test.nl");

            //Assert
            Assert.Equal("Packet already reserved", result);
        }

        //GetCantines
        [Fact]
        public async void Should_Return_All_Canteens() {
            //Arrange
            var canteen = new Cantine() { location = "" };
            var canteen2 = new Cantine() { location = "" };
            var canteen3 = new Cantine() { location = "" };
            _context.canteen.Add(canteen);
            _context.canteen.Add(canteen2);
            _context.canteen.Add(canteen3);

            _context.canteenStaffMembers.Add(new CantineStaffMember() { cantine = canteen, name="" });
            await _context.SaveChangesAsync();

            //Act
            var list = repository.GetCantines(1);

            //Assert
            Assert.NotNull(list);
            Assert.Equal(3, _context.canteen.Count());
        }

        [Fact]
        public async void Should_Return_All_Canteens_With_Canteen_Of_User_First() {
            //Arrange
             
            var canteen = new Cantine() { location = "" };
            var canteen2 = new Cantine() { location = "" };
            var canteen3 = new Cantine() { location = "" };
            _context.canteen.Add(canteen);
            _context.canteen.Add(canteen2);
            _context.canteen.Add(canteen3);

            _context.canteenStaffMembers.Add(new CantineStaffMember() { cantine = canteen, name = "" });
            await _context.SaveChangesAsync();

            //Act
            var list = repository.GetCantines(1);

            //Assert
            Assert.NotNull(list);
            Assert.Equal(canteen, _context.canteenStaffMembers.First().cantine);
            Assert.Equal(3, _context.canteen.Count());
        }

        //hasReservedForSpecificDay
        [Fact]
        public async void Has_Reserved_For_Specific_Day_With_Date_Null_Should_Return_False() {
            //Arrange
            Packet packet = new Packet() { name = "", reservedBy="test@test.com", id = 4, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var hasReserved = repository.hasReservedForSpecificDay(null, "test@test.com");

            //Assert
            Assert.False(hasReserved);
        }

        [Fact]
        public async void Has_Reserved_For_Specific_Day_With_Date_Where_Is_Reserved_Should_Return_True() {
            //Arrange
            Packet packet = new Packet() { name = "", reservedBy = "test@test.com", id = 4, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var hasReserved = repository.hasReservedForSpecificDay(DateTime.Now, "test@test.com");

            //Assert
            Assert.True(hasReserved);
        }

        [Fact]
        public async void Has_Reserved_For_Specific_Day_With_Date_Where_Is_Not_Reserved_Should_Return_False() {
            //Arrange
            Packet packet = new Packet() { name = "", reservedBy = "test@test.com", id = 4, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var hasReserved = repository.hasReservedForSpecificDay(DateTime.Now.AddDays(1), "test@test.com");

            //Assert
            Assert.False(hasReserved);
        }

        //GetExampleProducts
        [Fact]
        public async void Get_Example_Products_With_Type_Of_Meal_Is_Null_Returns_Null() {
            //Arrange
            var list = new ExampleProductList() {
                type = TypeOfMeal.Bread,
                list = new List<Product>() {
                    new Product() {name = "", alcoholic = false},
                    new Product() {name = "", alcoholic = false},
                    new Product() {name = "", alcoholic = false}
                }
            };
            _context.exampleProductLists.Add(list);
            await _context.SaveChangesAsync();

            //Act
            var result = repository.GetExampleProducts(null);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async void Get_Example_Products_With_Type_Of_Meal_Is_Returns_Products() {
            //Arrange
            var list = new ExampleProductList() {
                type = TypeOfMeal.Bread,
                list = new List<Product>() {
                    new Product() {name = "", alcoholic = false},
                    new Product() {name = "", alcoholic = false},
                    new Product() {name = "", alcoholic = false}
                }
            };
            _context.exampleProductLists.Add(list);
            await _context.SaveChangesAsync();

            //Act
            var result = repository.GetExampleProducts(TypeOfMeal.Bread);

            //Assert
            Assert.Equal(list, result);
        }
    }
}
