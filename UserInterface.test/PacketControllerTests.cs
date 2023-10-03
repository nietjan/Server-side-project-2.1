using DomainModel.enums;
using DomainModel;
using DomainServices;
using Infrastructure;
using UserInterface.Controllers;
using Microsoft.AspNetCore.Mvc;
using UserInterface.ViewComponents;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;
using System;
using NSubstitute.ReturnsExtensions;

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
            var result = sut.List(1) as ViewResult;

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
            Assert.Null(result.ViewName);
            Assert.NotNull(result.Model);
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
            Assert.Null(result.ViewName);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public void Canteen_Content_With_Invalid_Id_Should_Redirect() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();
            repoMock.GetCantines(1).Returns(new List<Cantine>() { InMemoryRepository.cantine, InMemoryRepository.cantine, InMemoryRepository.cantine }.AsQueryable());
            repoMock.GetPacketsOfCantine(-1).ReturnsNull();

            var sut = new PacketController(repoMock);

            //Act
            var result = sut.CanteenContents(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Home", result?.ControllerName);
        }

        [Fact]
        public void Register_Get_Should_Return_View() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();

            var sut = new PacketController(repoMock);

            //Act
            var result = sut.Register() as ViewResult;

            //Assert
            Assert.Null(result.ViewName); 
        }

        [Fact]
        public async void Register_Post_With_Valid_ModelState_Should_Return_Home() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();

            var sut = new PacketController(repoMock);
            var packet = new DomainModel.Packet() {
                name = "test",
                city = City.Breda,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddDays(1),
                typeOfMeal = TypeOfMeal.Drink,
                price = 1,
                eighteenUp = true,
                cantine = Infrastructure.InMemoryRepository.cantine,
            };
            repoMock.AddPacket(packet).Returns(true);

            //Act
            var result = await sut.Register(packet) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Home", result?.ControllerName);
        }

        [Fact]
        public async void Register_Post_With_InValid_ModelState_Should_Return_View() {
            //Arrange
            var repoMock = Substitute.For<IRepository>();

            var sut = new PacketController(repoMock);
            var packet = new DomainModel.Packet() {
                name = "test",
                city = City.Breda,
                startPickup = DateTime.Now,
                price = 1,
                endPickup = DateTime.Now.AddDays(1),
                typeOfMeal = TypeOfMeal.Drink,
                eighteenUp = true,
                cantine = Infrastructure.InMemoryRepository.cantine,
            };
            repoMock.AddPacket(packet).Returns(true);
            sut.ModelState.AddModelError("key", "error message");

            //Act
            var result = await sut.Register(packet) as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
        }
    }
}