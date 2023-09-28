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

        //GetAxampleProducts
        [Fact]
        public void Get_Only_Packets_Wich_Are_Not_Reserved() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = repository.GetPackets();

            //Assert
            Assert.DoesNotContain(repository.packets.ElementAt(2) ,products);
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
        [Fact]
        public async void Reserve_Packets_With_Correct_Info_Returns_True() {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = await repository.reservePacket(1,"test@test.nl");

            //Assert
            Assert.True(products);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public async void Reserve_Packets_With_Incorrect_Info_Returns_False(int id) {
            //Arrange
            InMemoryRepository repository = new InMemoryRepository();

            //Act
            var products = await repository.reservePacket(id, "test@test.nl");

            //Assert
            Assert.False(products);
        }

    }
}