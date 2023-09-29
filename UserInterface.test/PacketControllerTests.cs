using DomainModel.enums;
using DomainModel;
using DomainServices;
using Infrastructure;
using UserInterface.Controllers;
using Microsoft.AspNetCore.Mvc;
using UserInterface.ViewComponents;

namespace UserInterface.test {
    public class PacketControllerTests {
        private readonly List<Packet> packets = new List<Packet>() {
        new Packet() {
                id = 1,
                name = "Packet1",
                cantine = InMemoryRepository.cantine,
                city = InMemoryRepository.cantine.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Diner,
                price = 8,
                eighteenUp = true,
                reservedBy = "test@test.com"
            },
            new Packet() {
                id = 2,
                name = "Packet2",
                cantine = InMemoryRepository.cantine,
                city = InMemoryRepository.cantine.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Drink,
                price = 5,
                eighteenUp = true,
                reservedBy = "test@test.com"
            },new Packet() {
                id = 3,
                name = "Packet3",
                cantine = InMemoryRepository.cantine,
                city = InMemoryRepository.cantine.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Bread,
                price = 10,
                eighteenUp = true,
                reservedBy = "test@test.com"
            } 
        };

        [Fact]
        public void List_Shoud_Return_List_View() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();
            repoMock.GetReservedPackets("test@test.com").Returns((packets).AsQueryable());

            var sut = new PacketController(repoMock);

            //Act
            var result = sut.List() as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public void Reserved_Should_Return_List_View() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();
            repoMock.GetReservedPackets("test@test.com").Returns((packets).AsQueryable());

            var sut = new PacketController(repoMock);

            //Act
            var result = sut.Reserved() as ViewResult;

            //Assert
            Assert.Equal("List", result.ViewName);
            Assert.NotNull(result.Model);
        }

        //CanteenContents
        [Fact]
        public void Canteen_Content_With_Id_Should_Return_View() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();
            repoMock.GetPacketsOfCantine(1).Returns((packets).AsQueryable());
            repoMock.GetCantines(1).Returns(new List<Cantine>() { InMemoryRepository.cantine, InMemoryRepository.cantine, InMemoryRepository.cantine}.AsQueryable());

            var sut = new PacketController(repoMock);

            //Act
            var result = sut.CanteenContents(1) as ViewResult;

            //Assert
            Assert.Null(result?.ViewName);
            Assert.NotNull(result?.Model);
        }

        [Fact]
        public void Canteen_Content_Without_Id_Should_Return_View() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();
            repoMock.GetPacketsOfCantine(1).Returns((packets).AsQueryable());
            repoMock.GetCantines(1).Returns(new List<Cantine>() { InMemoryRepository.cantine, InMemoryRepository.cantine, InMemoryRepository.cantine }.AsQueryable());

            var sut = new PacketController(repoMock);

            //Act
            var result = sut.CanteenContents(0) as ViewResult;

            //Assert
            Assert.Null(result?.ViewName);
            Assert.NotNull(result?.Model);
        }

        [Fact]
        public void Canteen_Content_With_Invalid_Id_Should_Redirect() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();
            repoMock.GetPacketsOfCantine(1).Returns((packets).AsQueryable());
            repoMock.GetCantines(1).Returns(new List<Cantine>() { InMemoryRepository.cantine, InMemoryRepository.cantine, InMemoryRepository.cantine }.AsQueryable());

            var sut = new PacketController(repoMock);

            //Act
            var result = sut.CanteenContents(-1) as RedirectToActionResult;

            //Assert
            Assert.Null(result?.RouteValues?["action"]);
            Assert.Null(result?.RouteValues?["controller"]); // means we redirected to the same controller
        }
    }
}