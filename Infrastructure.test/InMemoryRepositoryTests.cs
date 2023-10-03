using DomainModel.enums;
using DomainModel;
using Infrastructure;
using System.ComponentModel;

namespace Infrastructure.test {
    public class InMemoryRepositoryTests {
        //Add Packet
        [Fact]
        public async void Packet_Without_Id_Should_Be_Added() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();
            var packet = new Packet() {
                name = "Packet2",
                cantine = InMemoryRepository.cantine,
                city = InMemoryRepository.cantine.city,
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
                cantine = InMemoryRepository.cantine,
                city = InMemoryRepository.cantine.city,
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
                cantine = InMemoryRepository.cantine,
                city = InMemoryRepository.cantine.city,
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
                cantine = InMemoryRepository.cantine,
                city = InMemoryRepository.cantine.city,
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

        //GetAxampleProducts
        [Fact]
        public void Axample_Product_Should_Be_Returned_When_Given_Valid_Id() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetAxampleProducts(1);

            //Assert
            Assert.NotNull(products);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void Axample_Product_Should_Return_0_When_Given_InValid_Id(int id) {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetAxampleProducts(id);

            //Assert
            Assert.Null(products);
        }

        //GetPackets
        [Fact]
        public void Get_Only_Packets_Wich_Are_Not_Reserved() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetPackets();

            //Assert
            Assert.DoesNotContain(repository.packets.ElementAt(2) ,products);
        }

        //GetPacketsOfCanteen
        [Fact]
        public void Get_All_Packets_Of_Canteen_With_Valid_Id() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var packets = repository.GetPacketsOfCantine(1);

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
            var packets = repository.GetPacketsOfCantine(id);

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
        public void Get_Single_Product_With_Correct_Id(int id) {
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
        public void Get_Single_Product_With_Incorrect_Id_That_Is_Null(int id) {
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
            var result = await repository.reservePacket(id,"test@test.nl");

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
            var result = await repository.reservePacket(id, "test@test.nl");

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
            var result = await repository.reservePacket(id, "test@test.nl");

            //Assert
            Assert.Equal("Packet not found", result);
        }

        [Fact]
        public async void Reserve_Packet_With_Id_Of_Already_Reserved_Package_Should_Return_Package_Already_Reserved() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var result = await repository.reservePacket(3, "test@test.nl");

            //Assert
            Assert.Equal("Packet already reserved", result);
        }

        //GetCantines
        [Fact]
        public void Should_Return_All_Cantines() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var list = repository.GetCantines(1);

            //Assert
            Assert.NotNull(list);
        }
    }
}