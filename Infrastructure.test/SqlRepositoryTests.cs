﻿using DomainModel.enums;
using DomainModel;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Infrastructure.test {
    public class SqlRepositoryTests : IDisposable {
        private PacketContext _context { get; set; }
        private SqlRepository repository { get; set; }

        public SqlRepositoryTests() {
            var optionsBuilder = new DbContextOptionsBuilder<PacketContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "AuthorDb");
            _context = new PacketContext(optionsBuilder.Options);
            var sessionMock = Substitute.For<ApplicationServices.IUserSession>();
            sessionMock.GetUserIdentityId().Returns("test");
            repository = new SqlRepository(_context, sessionMock);
    }
        public void Dispose() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

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
            Packet packet3 = new Packet() { name = "", reservedBy=new Student() { name="", studentNumber=123, studyCity = City.Breda}, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            
            _context.Add(packet);
            _context.Add(packet2);
            _context.Add(packet3);
            await _context.SaveChangesAsync();

            //Act
            var products = repository.GetPackets();

            //Assert
            Assert.DoesNotContain(packet3, products);
        }

        [Fact]
        public async void Get_Only_Packets_Which_Are_Not_Reserved_With_Filter_City() {
            //Arrange
            Packet packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet2 = new Packet() { name = "", city = City.Tilburg, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet3 = new Packet() { name = "", reservedBy = new Student() { name = "", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };

            _context.Add(packet);
            _context.Add(packet2);
            _context.Add(packet3);
            await _context.SaveChangesAsync();

            //Act
            var products = repository.GetPackets(city: City.Breda);

            //Assert
            Assert.DoesNotContain(packet2, products);
            Assert.DoesNotContain(packet3, products);
        }

        [Fact]
        public async void Get_Only_Packets_Which_Are_Not_Reserved_With_Filter_City_That_Is_Not_Used() {
            //Arrange
            Packet packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet2 = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet3 = new Packet() { name = "", reservedBy = new Student() { name = "", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };

            _context.Add(packet);
            _context.Add(packet2);
            _context.Add(packet3);
            await _context.SaveChangesAsync();

            //Act
            var products = repository.GetPackets(city: City.Tilburg);

            //Assert
            Assert.Empty(products);
        }

        [Fact]
        public async void Get_Only_Packets_Which_Are_Not_Reserved_With_Filter_Type_Of_Meal() {
            //Arrange
            Packet packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet2 = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Drink, price = 1 };
            Packet packet3 = new Packet() { name = "", reservedBy = new Student() { name = "", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };

            _context.Add(packet);
            _context.Add(packet2);
            _context.Add(packet3);
            await _context.SaveChangesAsync();

            //Act
            var products = repository.GetPackets(typeOfMeal: TypeOfMeal.Diner);

            //Assert
            Assert.Equal(packet, products.Single());
            Assert.DoesNotContain(packet2, products);
            Assert.DoesNotContain(packet3, products);
        }

        [Fact]
        public async void Get_Only_Packets_Which_Are_Not_Reserved_With_Filter_Type_Of_Meal_That_Is_Not_Used() {
            //Arrange
            Packet packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet2 = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            Packet packet3 = new Packet() { name = "", reservedBy = new Student() { name = "", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };

            _context.Add(packet);
            _context.Add(packet2);
            _context.Add(packet3);
            await _context.SaveChangesAsync();

            //Act
            var products = repository.GetPackets(typeOfMeal: TypeOfMeal.Bread);

            //Assert
            Assert.Empty(products);
        }


        //GetPacketsOfCanteen
        [Fact]
        public async void Get_All_Packets_Of_Canteen_With_Valid_Id() {
            //Arrange
            _context.canteen.Add(new Canteen() {
                location = "",
                city = City.Breda,
                id = 1,
                servesHotMeals = false,
                packetList = new List<Packet>() {
                    new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },
                    new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },
                    new Packet() { name = "", reservedBy = new Student() { name = "", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },

                }
            });
            await _context.SaveChangesAsync();   

            //Act
            var packets = repository.GetPacketsOfCanteen(1);

            //Assert
            Assert.NotNull(packets);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async void Get_All_Packets_Of_Canteen_With_Invalid_Id_Returns_Null(int id) {
            //Arrange
            _context.canteen.Add(new Canteen() {
                location = "",
                city = City.Breda,
                id = 1,
                servesHotMeals = false,
                packetList = new List<Packet>() {
                    new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },
                    new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },
                    new Packet() { name = "", reservedBy = new Student() { name = "", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 },

                }
            });
            await _context.SaveChangesAsync();


            //Act
            var packets = repository.GetPacketsOfCanteen(id);

            //Assert
            Assert.Null(packets);
        }

        //GetReservedPackets
        [Fact]
        public async void Get_Packets_Whom_Are_Reserved_With_Correct_Email() {
            //Arrange
            var packet = new Packet() { name = "", reservedBy = new Student() { name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
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
            _context.packets.Add(new Packet() { name = "", reservedBy = new Student() { name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 });
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
            var packet = new Packet() { name = "", id=1, reservedBy = new Student() { name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
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
            var packet = new Packet() { name = "", reservedBy = new Student() { name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
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
            var student = new Student() { securityId = "234324234", birthday=new DateTime(2005, 1, 1), name = "", studentNumber = 123, studyCity = City.Breda };
            _context.packets.Add(packet);
            _context.students.Add(student);
            
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.ReservePacket(packet.id, student.securityId);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async void Reserve_Packet_With_Not_Existing_Student_Should_Return_Student_Cannot_Be_Found() {
            //Arrange
            var packet = new Packet() { name = "", city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);

            await _context.SaveChangesAsync();

            //Act
            var result = await repository.ReservePacket(packet.id, "23434");

            //Assert
            Assert.Equal("Student cannot be found", result);
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
            var result = await repository.ReservePacket(id, "test@test.nl");

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
            var result = await repository.ReservePacket(id, "test@test.nl");

            //Assert
            Assert.Equal("Packet not found", result);
        }

        [Fact]
        public async void Reserve_Packet_With_Id_Of_Already_Reserved_Package_Should_Return_Package_Already_Reserved() {
            //Arrange
            var packet = new Packet() { name = "", id=1, reservedBy = new Student() {securityId= "test@test.com", name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.ReservePacket(1, "test@test.nl");

            //Assert
            Assert.Equal("Packet already reserved", result);
        }


        [Fact]
        public async void Reserve_Packet_With_Student_Not_Old_Enough_Should_Return_Student_Not_Old_Enough_To_Reserve_Packet() {
            //Arrange
            var packet = new Packet() { name = "", id = 1, eighteenUp=true, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            var student = new Student() { securityId = "234324234", birthday = DateTime.Now.AddYears(-16), name = "", studentNumber = 123, studyCity = City.Breda };
            _context.packets.Add(packet);
            _context.students.Add(student);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.ReservePacket(1, student.securityId);

            //Assert
            Assert.Equal("Student not old enough to reserve packet", result);
        }


        //unreservePacket
        [Fact]
        public async void Unreserve_Packet_With_Id_Of_Not_Reserved_Package_Should_Return_Package_Was_Not_Reserved() {
            //Arrange
            var packet = new Packet() { name = "", id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.UnreservePacket(1, "test@test.nl");

            //Assert
            Assert.Equal("Packet was not reserved", result);
        }

        [Fact]
        public async void Unreserve_Packet_With_Id_Of_Not_Existing_Packet_Should_Return_Package_Not_Found() {
            //Arrange

            //Act
            var result = await repository.UnreservePacket(1, "test@test.nl");

            //Assert
            Assert.Equal("Packet not found", result);
        }

        [Fact]
        public async void Unreserve_Packet_With_Id_Of_Package_Reserved_By_Another_User_Should_Return_Package_Is_Not_Reserved_By_User() {
            //Arrange
            var packet = new Packet() {reservedBy = new Student() { securityId = "test@test.com", name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, name = "", id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.UnreservePacket(1, "DifferentString");

            //Assert
            Assert.Equal("Packet is not reserved by user", result);
        }

        [Fact]
        public async void Unreserve_Packet_With_Id_Of_Package_That_Is_Reserved_By_User_Should_Return_Null() {
            //Arrange
            var packet = new Packet() { reservedBy = new Student() { securityId = "test@test.com", name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, name = "", id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.UnreservePacket(1, "test@test.com");

            //Assert
            Assert.Null(result);
        }

        //GetCanteens
        [Fact]
        public async void Should_Return_All_Canteens() {
            //Arrange
            var canteen = new Canteen() { location = "" };
            var canteen2 = new Canteen() { location = "" };
            var canteen3 = new Canteen() { location = "" };
            _context.canteen.Add(canteen);
            _context.canteen.Add(canteen2);
            _context.canteen.Add(canteen3);

            _context.canteenStaffMembers.Add(new CanteenStaffMember() {securityId="1", canteen = canteen, name="" });
            await _context.SaveChangesAsync();

            //Act
            var list = repository.GetCanteens("1");

            //Assert
            Assert.NotNull(list);
            Assert.Equal(3, _context.canteen.Count());
        }

        [Fact]
        public async void Should_Return_All_Canteens_With_Canteen_Of_User_First() {
            //Arrange
             
            var canteen = new Canteen() { location = "" };
            var canteen2 = new Canteen() { location = "" };
            var canteen3 = new Canteen() { location = "" };
            _context.canteen.Add(canteen);
            _context.canteen.Add(canteen2);
            _context.canteen.Add(canteen3);

            _context.canteenStaffMembers.Add(new CanteenStaffMember() { securityId = "1", canteen = canteen, name = "" });
            await _context.SaveChangesAsync();

            //Act
            var list = repository.GetCanteens("1");

            //Assert
            Assert.NotNull(list);
            Assert.Equal(canteen, _context.canteenStaffMembers.First().canteen);
            Assert.Equal(3, _context.canteen.Count());
        }

        //hasReservedForSpecificDay
        [Fact]
        public async void Has_Reserved_For_Specific_Day_With_Date_Null_Should_Return_False() {
            //Arrange
            Packet packet = new Packet() { name = "", reservedBy=new Student() { name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, id = 4, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var hasReserved = repository.HasReservedForSpecificDay(null, "test@test.com");

            //Assert
            Assert.False(hasReserved);
        }

        [Fact]
        public async void Has_Reserved_For_Specific_Day_With_Date_Where_Is_Reserved_Should_Return_True() {
            //Arrange
            Packet packet = new Packet() { name = "", reservedBy = new Student() {securityId="34675345345", name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, id = 4, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var hasReserved = repository.HasReservedForSpecificDay(DateTime.Now, "34675345345");

            //Assert
            Assert.True(hasReserved);
        }

        [Fact]
        public async void Has_Reserved_For_Specific_Day_With_Date_Where_Is_Not_Reserved_Should_Return_False() {
            //Arrange
            Packet packet = new Packet() { name = "", reservedBy = new Student() { name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, id = 4, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var hasReserved = repository.HasReservedForSpecificDay(DateTime.Now.AddDays(1), "test@test.com");

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

        //Update packet
        [Fact]
        public async void Update_Packet_With_Not_In_Db_Packet_Returns_False() {
            //Arrange
            _context.Add(new Packet() { name = "", id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 });
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.UpdatePacket(new Packet() { name = "", id = 2, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 });

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async void Update_Packet_With_Packet_In_Db_Packet_Returns_True() {
            //Arrange
            Packet packet = new Packet() { name = "", id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();
            packet.name = "sss";

            //Act
            var result = await repository.UpdatePacket(packet);

            //Assert
            Assert.True(result);
            Assert.Equal(packet.name, _context.packets.First().name);
        }

        //Delete packet
        [Fact]
        public async void Delete_Packet_With_Not_In_Db_Packet_Returns_False() {
            //Arrange

            //Act
            var result = await repository.DeletePacket(new Packet() {
                name = " ",
                id = 0,
            });

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async void Delete_Packet_With_Packet_In_Db_Packet_Returns_False_When_Reserved() {
            //Arrange
            Packet packet = new Packet() { name = "", reservedBy = new Student() { name="", studentNumber=134, studyCity=City.Breda}, id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.DeletePacket(packet);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async void Delete_Packet_With_Packet_In_Db_Packet_Returns_True_When_Not_Reserved() {
            Packet packet = new Packet() { name = "",  id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            _context.packets.Add(packet);
            await _context.SaveChangesAsync();

            //Act
            var result = await repository.DeletePacket(packet);

            //Assert
            Assert.True(result);
        }

        //GetCanteen
        [Fact]
        public async void Get_Canteen_With_Valid_Id_Should_Return_Canteen() {
            //Arrange
            string securityId = "123";
            var canteen = new Canteen() { location = "" };
            var staffMember = new CanteenStaffMember() { name = "", securityId = securityId, staffNumber = 123, canteen = canteen };
            _context.canteenStaffMembers.Add(staffMember);
            _context.canteen.Add(canteen);
            await _context.SaveChangesAsync();

            //Act
            var result = repository.GetCanteen(securityId);

            //Assert
            Assert.Equal(canteen, result);
        }

        [Fact]
        public async void Get_Canteen_With_Invalid_Id_Should_Return_Null() {
            //Arrange
            string securityId = "123";
            var canteen = new Canteen() { location = "" };
            var staffMember = new CanteenStaffMember() { name = "", securityId = securityId, staffNumber = 123, canteen = canteen };
            _context.canteenStaffMembers.Add(staffMember);
            _context.canteen.Add(canteen);
            await _context.SaveChangesAsync();

            //Act
            var result = repository.GetCanteen("");

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async void Get_Canteen_With_Canteen_Equals_Null_Should_Return_Null() {
            //Arrange
            string securityId = "123";
            var staffMember = new CanteenStaffMember() { name = "", securityId = securityId, staffNumber = 123 };
            _context.canteenStaffMembers.Add(staffMember);
            await _context.SaveChangesAsync();

            //Act
            var result = repository.GetCanteen("");

            //Assert
            Assert.Null(result);
        }

        //userIsCanteenStaff
        [Fact]
        public async void User_Is_Canteen_Staff_With_Valid_Id_Should_Return_True() {
            //Arrange
            string securityId = "123";
            var staffMember = new CanteenStaffMember() { name = "", securityId = securityId, staffNumber = 123 };
            _context.canteenStaffMembers.Add(staffMember);
            await _context.SaveChangesAsync();

            //Act
            var result = repository.UserIsCanteenStaff(securityId);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async void User_Is_Canteen_Staff_With_Invalid_Id_Should_Return_False() {
            //Arrange
            string securityId = "123";

            //Act
            var result = repository.UserIsCanteenStaff(securityId);

            //Assert
            Assert.False(result);
        }

        //GetStudent
        [Fact]
        public async void Get_Student_With_Valid_Id_Should_Return_Student() {
            //Arrange
            var student = new Student() { securityId = "234324234", birthday = DateTime.Now.AddYears(-16), name = "", studentNumber = 123, studyCity = City.Breda };
            _context.students.Add(student);
            await _context.SaveChangesAsync();

            //Act
            var result = repository.GetStudent(student.securityId);

            //Assert
            Assert.Equal(student, result);
        }

        [Fact]
        public async void Get_Student_With_Nonvalid_Id_Should_Return_Null() {
            //Arrange
            var student = new Student() { securityId = "234324234", birthday = DateTime.Now.AddYears(-16), name = "", studentNumber = 123, studyCity = City.Breda };
            _context.students.Add(student);
            await _context.SaveChangesAsync();

            //Act
            var result = repository.GetStudent("test");

            //Assert
            Assert.Null(result);
        }

        //GetAllPackets
        [Fact]
        public void Get_All_Packets_With_No_Packets_Should_Return_Empty_List() {
            //Arrange

            //Act
            var result = repository.GetAllPackets();

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void Get_All_Packets_With_Packets_Should_Return_List_With_Packets() {
            //Arrange
            Packet[] packets = {
                new Packet(){name = "packet1", city=City.Breda, price=5, typeOfMeal = TypeOfMeal.Bread, startPickup=DateTime.Now.AddDays(10), endPickup=DateTime.Now.AddDays(11)},
                new Packet(){name = "packet2", city=City.Breda, price=15, typeOfMeal = TypeOfMeal.Diner, startPickup=DateTime.Now.AddDays(11), endPickup=DateTime.Now.AddDays(11).AddHours(5)}
            };
            _context.packets.AddRange(packets);
            await _context.SaveChangesAsync();


            //Act
            var result = repository.GetAllPackets();

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(packets[0], result.First());
        }

        //GetAllProducts
        [Fact]
        public void Get_All_Products_With_No_Products_Should_Return_Empty_List() {
            //Arrange

            //Act
            var result = repository.GetAllProducts();

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void Get_All_Products_With_Products_Should_Return_List_With_Packets() {
            //Arrange
            Product[] productsList = {
                    new Product() {alcoholic=true, name="bread1" },
                    new Product() {alcoholic=true, name="bread2" },
                    new Product() {alcoholic=false, name="bread3"},

                    new Product() {alcoholic=true, name="diner1"},
                    new Product() {alcoholic=false, name="diner2"},
                    new Product() {alcoholic=false, name="diner3"},

                    new Product() {alcoholic=false, name="drink1"},
                    new Product() {alcoholic=false, name="drink2"},
                    new Product() {alcoholic=false, name="drink3"},
            };
            _context.products.AddRange(productsList);
            await _context.SaveChangesAsync();


            //Act
            var result = repository.GetAllProducts();

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(productsList[0], result.First());
        }
    }
}
