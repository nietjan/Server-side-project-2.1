using DomainModel.enums;
using DomainModel;
using Infrastructure;
using System.ComponentModel;
using Xunit;
using DomainServices;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.test {
    public class InMemoryRepositoryTests {
        //Add Packet
        [Fact]
        public async void Packet_Without_Id_Should_Be_Added() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = new Packet() {
                name = "Packet2",
                canteen = InMemoryRepository.Canteen,
                city = InMemoryRepository.Canteen.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Drink,
                price = 5,
                eighteenUp = true
            };

            //Act
            await repository.AddPacket(packet);

            //Assert
            Assert.Equal(packet, repository.packets.Last());
        }


        [Fact]
        public async void Packet_Without_Id_Gets_Id_When_Added() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = new Packet() {
                name = "Packet2",
                canteen = InMemoryRepository.Canteen,
                city = InMemoryRepository.Canteen.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Drink,
                price = 5,
                eighteenUp = true
            };

            //Act
            await repository.AddPacket(packet);

            //Assert
            Assert.NotEqual(0, repository.packets.Last().id);
        }

        [Fact]
        public async void Packet_With_Id_Keeps_Id_When_Added() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = new Packet() {
                id = 4,
                name = "Packet2",
                canteen = InMemoryRepository.Canteen,
                city = InMemoryRepository.Canteen.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Drink,
                price = 5,
                eighteenUp = true
            };

            //Act
            await repository.AddPacket(packet);

            //Assert
            Assert.Equal(4, repository.packets.Last().id);
        }


        [Fact]
        public async void Packet_With_Id_0_Gets_Different_Id_When_Added() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = new Packet() {
                id = 0,
                name = "Packet2",
                canteen = InMemoryRepository.Canteen,
                city = InMemoryRepository.Canteen.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Drink,
                price = 5,
                eighteenUp = true
            };

            //Act
            await repository.AddPacket(packet);

            //Assert
            Assert.NotEqual(0, repository.packets.Last().id);
        }

        //GetPackets
        [Fact]
        public void Get_Only_Packets_Which_Are_Not_Reserved() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetPackets();

            //Assert
            Assert.DoesNotContain(repository.packets.ElementAt(2) ,products);
        }

        [Fact]
        public void Get_Only_Packets_Which_Are_Not_Reserved_With_Filter_City() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetPackets(city: City.Breda);

            //Assert
            Assert.DoesNotContain(repository.packets.ElementAt(2), products);
        }

        [Fact]
        public void Get_Only_Packets_Which_Are_Not_Reserved_With_Filter_City_That_Is_Not_Used() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetPackets(city: City.Tilburg);

            //Assert
            Assert.Empty(products);
        }

        [Fact]
        public void Get_Only_Packets_Which_Are_Not_Reserved_With_Filter_Type_Of_Meal() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetPackets(typeOfMeal: TypeOfMeal.Drink);

            //Assert
            Assert.Equal(repository.packets.ElementAt(1), products.Single());
        }

        [Fact]
        public void Get_Only_Packets_Which_Are_Not_Reserved_With_Filter_Type_Of_Meal_That_Is_Not_Used() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetPackets(typeOfMeal: TypeOfMeal.Bread);

            //Assert
            Assert.Empty(products);
        }


        //GetPacketsOfCanteen
        [Fact]
        public void Get_All_Packets_Of_Canteen_With_Valid_Id() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var packets = repository.GetPacketsOfCanteen(1);

            //Assert
            Assert.NotNull(packets);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void Get_All_Packets_Of_Canteen_With_Invalid_Id_Returns_Null(int id) {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var packets = repository.GetPacketsOfCanteen(id);

            //Assert
            Assert.Null(packets);
        }

        //GetReservedPackets
        [Fact]
        public void Get_Packets_Wich_Are_Reserved_With_Correct_Email() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetReservedPackets("test@test.com");

            //Assert
            Assert.Contains(repository.packets.ElementAt(2), products);
        }

        [Fact]
        public void Get_List_Of_Packets_Wich_Are_Reserved_With_InCorrect_Email_That_Is_Empty() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetReservedPackets("test2@test2.nl");

            //Assert
            Assert.Empty(products);
        }

        //GetAxampleProducts
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Get_Single_Packet_With_Correct_Id(int id) {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetSinglePacket(id);

            //Assert
            Assert.NotNull(products);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void Get_Single_Packet_With_Incorrect_Id_That_Is_Null(int id) {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetSinglePacket(id);

            //Assert
            Assert.Null(products);
        }

        //Reserve packet
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void Reserve_Packet_With_Correct_Info_Should_Return_Null(int id) {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.ReservePacket(id, "99aed502-3fab-4077-acca-d74391cecd3f");

            //Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public async void Reserve_Packet_With_Id_Lower_Or_Equal_To_0_Should_Return_Package_Not_Found(int id) {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.ReservePacket(id, "test@test.nl");

            //Assert
            Assert.Equal("Packet not found", result);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(10)]
        public async void Reserve_Packet_With_Not_Existing_Id_Should_Return_Package_Not_Found(int id) {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.ReservePacket(id, "test@test.nl");

            //Assert
            Assert.Equal("Packet not found", result);
        }

        [Fact]
        public async void Reserve_Packet_With_Id_Of_Already_Reserved_Package_Should_Return_Package_Already_Reserved() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.ReservePacket(3, "test@test.nl");

            //Assert
            Assert.Equal("Packet already reserved", result);
        }

        [Fact]
        public async void Reserve_Packet_With_Student_Not_Old_Enough_Should_Return_Student_Not_Old_Enough_To_Reserve_Packet() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.ReservePacket(1, "test@test.nl");

            //Assert
            Assert.Equal("Student not old enough to reserve packet", result);
        }

        //UnreservePacket
        [Fact]
        public async void Unreserve_Packet_With_Id_Of_Not_Reserved_Package_Should_Return_Package_Was_Not_Reserved() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.UnreservePacket(1, "test@test.nl");

            //Assert
            Assert.Equal("Packet was not reserved", result);
        }

        [Fact]
        public async void Unreserve_Packet_With_Id_Of_Not_Existing_Packet_Should_Return_Package_Not_Found() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.UnreservePacket(10, "test@test.nl");

            //Assert
            Assert.Equal("Packet not found", result);
        }

        [Fact]
        public async void Unreserve_Packet_With_Id_Of_Package_Reserved_By_Another_User_Should_Return_Package_Is_Not_Reserved_By_User() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.UnreservePacket(3, "DifferentString");

            //Assert
            Assert.Equal("Packet is not reserved by user", result);
        }

        [Fact]
        public async void Unreserve_Packet_With_Id_Of_Package_That_Is_Reserved_By_User_Should_Return_Null() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = new Packet() { reservedBy = new Student() { securityId = "test@test.com", name = "test@test.com", studentNumber = 123, studyCity = City.Breda }, name = "", id = 10, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 };
            repository.packets.Add(packet);

            //Act
            var result = await repository.UnreservePacket(10, "test@test.com");

            //Assert
            Assert.Null(result);
        }

        //GetCanteens
        [Fact]
        public void Should_Return_All_Canteens() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var list = repository.GetCanteens("1");

            //Assert
            Assert.NotNull(list);
        }

        //hasReservedForSpecificDay
        [Fact]
        public void Has_Reserved_For_Specific_Day_With_Date_Null_Should_Return_False() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var hasReserved = repository.HasReservedForSpecificDay(null, "test@test.com");

            //Assert
            Assert.False(hasReserved);
        }

        [Fact]
        public void Has_Reserved_For_Specific_Day_With_Date_Where_Is_Reserved_Should_Return_True() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var hasReserved = repository.HasReservedForSpecificDay(DateTime.Now, "test@test.com");

            //Assert
            Assert.True(hasReserved);
        }

        [Fact]
        public void Has_Reserved_For_Specific_Day_With_Date_Where_Is_Not_Reserved_Should_Return_False() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var hasReserved = repository.HasReservedForSpecificDay(DateTime.Now.AddDays(1), "test@test.com");

            //Assert
            Assert.False(hasReserved);
        }

        //GetExampleProducts
        [Fact]
        public void Get_Example_Products_With_Type_Of_Meal_Is_Null_Returns_Null() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = repository.GetExampleProducts(null);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void Get_Example_Products_With_Type_Of_Meal_Is_Returns_Products() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = repository.GetExampleProducts(TypeOfMeal.Bread);

            //Assert
            Assert.Equal(InMemoryRepository.productsExampleList.First(), result);
        }

        //Update packet
        [Fact]
        public async void Update_Packet_With_Not_In_Db_Packet_Returns_False() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.UpdatePacket(new Packet(){
                name = " ",
                id = 0,
            });

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async void Update_Packet_With_Packet_In_Db_Packet_Returns_True() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = repository.packets.First();
            packet.price = 1;

            //Act
            var result = await repository.UpdatePacket(packet);

            //Assert
            Assert.True(result);
        }

        //Delete packet
        [Fact]
        public async void Delete_Packet_With_Not_In_Db_Packet_Returns_False() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.UpdatePacket(new Packet() {
                name = " ",
                id = 0,
            });

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async void Delete_Packet_With_Packet_In_Db_Packet_Returns_False_When_Reserved() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = repository.packets.Last();
     
            //Act
            var result = await repository.DeletePacket(packet);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async void Delete_Packet_With_Packet_In_Db_Packet_Returns_True_When_Not_Reserved() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = repository.packets.First();

            //Act
            var result = await repository.DeletePacket(packet);

            //Assert
            Assert.True(result);
        }

        //GetStudent
        [Fact]
        public void Get_Student_With_Valid_Id_Should_Return_Student() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = repository.GetStudent("99aed502-3fab-4077-acca-d74391cecd3f");

            //Assert
            Assert.Equal("99aed502-3fab-4077-acca-d74391cecd3f", result.securityId);
        }

        [Fact]
        public void Get_Student_With_Nonvalid_Id_Should_Return_Other_Student() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            Student student = new Student() { securityId = "test", name = "Student1", birthday = new DateTime(2006, 1, 1), studentNumber = 123, studyCity = DomainModel.enums.City.Breda };

            //Act
            var result = repository.GetStudent("test");

            //Assert
            Assert.Equal(student.id, result?.id);
        }

        //GetAllPackets
        [Fact]
        public async void Get_All_Packets_With_Packets_Should_Return_List_With_Packets() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = repository.GetAllPackets();

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(repository.packets[0], result.First());
        }

        //GetAllProducts
        [Fact]
        public async void Get_All_Products_With_Products_Should_Return_List_With_Packets() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = repository.GetAllProducts();

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(InMemoryRepository.productsExampleList[0].list.First(), result.First());
        }
    }
}