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
using ApplicationServices;
using Microsoft.EntityFrameworkCore;

namespace UserInterface.test {
    public class PacketControllerTests : IDisposable {
        private static readonly string securityId = "123";
        private readonly List<Packet> packets = new List<Packet>() {
        new Packet() {
                id = 1,
                name = "Packet1",
                canteen = InMemoryRepository.Canteen,
                city = InMemoryRepository.Canteen.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Diner,
                price = 8,
                eighteenUp = true,
                reservedBy = new Student() { name = "test@test.com", studentNumber = 123, studyCity = City.Breda }
            },
            new Packet() {
                id = 2,
                name = "Packet2",
                canteen = InMemoryRepository.Canteen,
                city = InMemoryRepository.Canteen.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Drink,
                price = 5,
                eighteenUp = true,
                reservedBy = new Student() { name = "test@test.com", studentNumber = 123, studyCity = City.Breda }
            },new Packet() {
                id = 3,
                name = "Packet3",
                canteen = InMemoryRepository.Canteen,
                city = InMemoryRepository.Canteen.city,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddHours(2),
                typeOfMeal = TypeOfMeal.Bread,
                price = 10,
                eighteenUp = true,
                reservedBy = new Student() {securityId=securityId, name = "test@test.com", studentNumber = 123, studyCity = City.Breda }
            } 
        };

        private IRepository repoMock { get; set; }
        private IUserSession userSessionMock { get; set; }
        private PacketController sut;

        public PacketControllerTests() {
            repoMock = Substitute.For<IRepository>();
            userSessionMock = Substitute.For<IUserSession>();
            userSessionMock.GetUserIdentityId().Returns(securityId);
            sut = new PacketController(repoMock, userSessionMock);
        }
        public void Dispose() {
            sut.Dispose();
        }

        [Fact]
        public void List_Shoud_Return_List_View_If_Student() {
            //Arrange
            repoMock.GetReservedPackets("test@test.com").Returns((packets).AsQueryable());
            repoMock.UserIsCanteenStaff(Arg.Any<string>()).Returns(false);

            //Act
            var result = sut.List() as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
            Assert.NotNull(result.Model);
        }

        public void List_Shoud_Redirect_If_Staff() {
            //Arrange
            repoMock.GetReservedPackets("test@test.com").Returns((packets).AsQueryable());
            repoMock.UserIsCanteenStaff(Arg.Any<string>()).Returns(true);

            //Act
            var result = sut.List() as RedirectToActionResult; ;

            //Assert
            Assert.Equal("CanteenContents", result?.ActionName);
        }

        [Fact]
        public void Reserved_Should_Return_List_View() {
            //Arrange
            repoMock.GetReservedPackets("test@test.com").Returns((packets).AsQueryable());

            //Act
            var result = sut.Reserved() as ViewResult;

            //Assert
            Assert.Equal("List", result.ViewName);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public void Canteen_Content_With_Id_Should_Return_View() {
            //Arrange
            repoMock.GetPacketsOfCanteen(Arg.Any<int>()).Returns((packets).AsQueryable());
            repoMock.GetCanteen(Arg.Any<string>()).Returns(InMemoryRepository.Canteen);

            //Act
            var result = sut.CanteenContents(1) as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public void Canteen_Content_Without_Id_Should_Return_View() {
            //Arrange
            repoMock.GetPacketsOfCanteen(1).Returns((packets).AsQueryable());
            repoMock.GetCanteen(Arg.Any<string>()).Returns(InMemoryRepository.Canteen);

            //Act
            var result = sut.CanteenContents(0) as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public void Canteen_Content_With_Invalid_Id_Should_Redirect() {
            //Arrange
            repoMock.GetCanteen(Arg.Any<string>()).Returns(InMemoryRepository.Canteen);
            repoMock.GetPacketsOfCanteen(Arg.Any<int>()).ReturnsNull();

            //Act
            var result = sut.CanteenContents(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Home", result?.ControllerName);
        }

        [Fact]
        public void Canteen_Content_With_Id_0_But_Canteen_Cannot_Be_Found_Redirect() {
            //Arrange
            repoMock.GetCanteen(Arg.Any<string>()).ReturnsNull();
            repoMock.GetPacketsOfCanteen(Arg.Any<int>()).ReturnsNull();

            //Act
            var result = sut.CanteenContents(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Home", result?.ControllerName);
        }

        [Fact]
        public void Register_Get_Should_Return_View() {
            //Arrange

            //Act
            var result = sut.Register() as ViewResult;

            //Assert
            Assert.Null(result.ViewName); 
        }

        [Fact]
        public async void Register_Post_With_Valid_ModelState_Should_Return_Home() {
            //Arrange
            var packet = new DomainModel.Packet() {
                name = "test",
                city = City.Breda,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddDays(1),
                typeOfMeal = TypeOfMeal.Drink,
                price = 1,
                eighteenUp = true,
                canteen = Infrastructure.InMemoryRepository.Canteen,
            };
            repoMock.GetCanteen(Arg.Any<string>()).Returns(InMemoryRepository.Canteen);
            repoMock.AddPacket(packet).Returns(true);

            //Act
            var result = await sut.Register(packet) as RedirectToActionResult;

            //Assert
            Assert.Equal("CanteenContents", result?.ActionName);
            Assert.Null(result?.ControllerName);
        }

        [Fact]
        public async void Register_Post_With_InValid_ModelState_Should_Return_View() {
            //Arrange
            var packet = new DomainModel.Packet() {
                name = "test",
                city = City.Breda,
                startPickup = DateTime.Now,
                price = 1,
                endPickup = DateTime.Now.AddDays(1),
                typeOfMeal = TypeOfMeal.Drink,
                eighteenUp = true,
                canteen = Infrastructure.InMemoryRepository.Canteen,
            };
            repoMock.AddPacket(packet).Returns(true);
            repoMock.GetCanteen(Arg.Any<string>()).Returns(InMemoryRepository.Canteen);
            sut.ModelState.AddModelError("key", "error message");

            //Act
            var result = await sut.Register(packet) as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
        }

        [Fact]
        public void Update_Get_Should_Return_View() {
            //Arrange
            repoMock.GetSinglePacket(1).Returns(new Packet(){ name = "", id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 });

            //Act
            var result = sut.Update(1) as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
        }

        [Fact]
        public void Update_Get_Should_Redirect_When_Packet_Does_Not_Exist() {
            //Arrange
            repoMock.GetSinglePacket(1).ReturnsNull();

            //Act
            var result = sut.Update(1) as RedirectToActionResult; ;

            //Assert
            Assert.Equal("CanteenContents", result?.ActionName);
            Assert.Equal("NotFound", result?.RouteValues?["Update"]);
        }

        [Fact]
        public void Update_Get_Should_Redirect_When_Packet_Is_Reserved() {
            //Arrange
            repoMock.GetSinglePacket(1).Returns(new Packet() { name = "", reservedBy=new Student() { name = "", studentNumber = 123, studyCity = City.Breda }, id = 1, city = City.Breda, startPickup = DateTime.Now, endPickup = DateTime.Now.AddDays(1), typeOfMeal = TypeOfMeal.Diner, price = 1 });

            //Act
            var result = sut.Update(1) as RedirectToActionResult; ;

            //Assert
            Assert.Equal("CanteenContents", result?.ActionName);
            Assert.Equal("Reserved", result?.RouteValues?["Update"]);
        }

        [Fact]
        public async void Update_Post_With_Valid_ModelState_Should_Return_Home() {
            //Arrange
            var packet = new DomainModel.Packet() {
                name = "test",
                city = City.Breda,
                startPickup = DateTime.Now,
                endPickup = DateTime.Now.AddDays(1),
                typeOfMeal = TypeOfMeal.Drink,
                price = 1,
                eighteenUp = true,
                canteen = Infrastructure.InMemoryRepository.Canteen,
            };
            repoMock.GetCanteen(Arg.Any<string>()).Returns(InMemoryRepository.Canteen);
            repoMock.UpdatePacket(Arg.Any<Packet>()).Returns(true);
            repoMock.UpdatePacket(packet).Returns(true);

            //Act
            var result = await sut.Update(packet) as RedirectToActionResult;

            //Assert
            Assert.Equal("Detail", result?.ActionName);
        }

        [Fact]
        public async void Update_With_InValid_ModelState_Should_Return_View() {
            //Arrange
            var packet = new DomainModel.Packet() {
                name = "test",
                city = City.Breda,
                startPickup = DateTime.Now,
                price = 1,
                endPickup = DateTime.Now.AddDays(1),
                typeOfMeal = TypeOfMeal.Drink,
                eighteenUp = true,
                canteen = Infrastructure.InMemoryRepository.Canteen,
            };
            repoMock.GetCanteen(Arg.Any<string>()).Returns(InMemoryRepository.Canteen);
            repoMock.UpdatePacket(packet).Returns(false);
            sut.ModelState.AddModelError("key", "error message");

            //Act
            var result = await sut.Update(packet) as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
        }

        [Fact]
        public async void Delete_With_Invalid_Id_Should_Redirect_With_Query_Param() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).ReturnsNull();
            sut.ModelState.AddModelError("key", "error message");

            //Act
            var result = await sut.Delete(1) as RedirectToActionResult;

            //Assert
            Assert.Equal("CanteenContents", result?.ActionName);
            Assert.Equal("NotFound", result?.RouteValues?["Delete"]);
        }

        [Fact]
        public async void Delete_With_Valid_Id_But_Already_Reserved_Package__Should_Redirect_With_Query_Param() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() {
                id = 1,
                reservedBy = new Student() { name = "", studentNumber = 123, studyCity = City.Breda },
                name = "test",
                city = City.Breda,
                startPickup = DateTime.Now,
                price = 1,
                endPickup = DateTime.Now.AddDays(1),
                typeOfMeal = TypeOfMeal.Drink,
                eighteenUp = true,
                canteen = InMemoryRepository.Canteen,
            });

            //Act
            var result = await sut.Delete(1) as RedirectToActionResult;

            //Assert
            Assert.Equal("CanteenContents", result?.ActionName);
            Assert.Equal("Reserved", result?.RouteValues?["Delete"]);
        }

        [Fact]
        public async void Delete_With_Valid_Id_Should_Redirect() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() {
                id = 1,
                reservedBy = new Student() { name = "", studentNumber = 123, studyCity = City.Breda },
                name = "test",
                city = City.Breda,
                startPickup = DateTime.Now,
                price = 1,
                endPickup = DateTime.Now.AddDays(1),
                typeOfMeal = TypeOfMeal.Drink,
                eighteenUp = true,
                canteen = InMemoryRepository.Canteen,
            });
            repoMock.DeletePacket(Arg.Any<Packet>()).Returns(true);

            //Act
            var result = await sut.Delete(1) as RedirectToActionResult;

            //Assert
            Assert.Equal("CanteenContents", result?.ActionName);
        }

        [Fact]
        public void Detail_With_Not_Correct_Id_Should_Redirect() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).ReturnsNull();

            //Act
            var result = sut.Detail(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Home", result?.ControllerName);
        }

        [Fact]
        public void Detail_With_Id_Of_Zero_Should_Redirect() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(packets.First());

            //Act
            var result = sut.Detail(0) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Home", result?.ControllerName);
        }

        [Fact]
        public void Detail_With_Correct_Id_Should_Show_View() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(packets.First());

            //Act
            var result = sut.Detail(1) as ViewResult;

            //Assert
            Assert.Null(result.ViewName);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public async Task Reserve_With_Id_Of_Not_Existing_Package_Should_Redirect_To_List_1() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).ReturnsNull();
            repoMock.ReservePacket(Arg.Any<int>(), Arg.Any<string>()).ReturnsNull();

            //Act
            var result = await sut.reservePacket(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("NotFound", result?.RouteValues?["Reserve"]);
            Assert.Null(result?.ControllerName);
        }

        [Fact]
        public async Task Reserve_With_Id_Of_Not_Existing_Package_Should_Redirect_To_List_2() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.ReservePacket(Arg.Any<int>(), Arg.Any<string>()).Returns("Packet not found");

            //Act
            var result = await sut.reservePacket(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("NotFound", result?.RouteValues?["Reserve"]);
            Assert.Null(result?.ControllerName);
        }

        [Fact]
        public async Task Reserve_With_Id_Of_Packet_Already_Reserved_Should_Redirect_To_List() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.ReservePacket(Arg.Any<int>(), Arg.Any<string>()).Returns("Packet already reserved");

            //Act
            var result = await sut.reservePacket(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("Reserved", result?.RouteValues?["Reserve"]);
            Assert.Null(result?.ControllerName);
        }

        [Fact]
        public async Task Reserve_With_Id_Of_Packet_When_Student_Has_Already_Reserved_A_Package_Should_Redirect_To_List() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.ReservePacket(Arg.Any<int>(), Arg.Any<string>()).Returns("Already reserved a package");

            //Act
            var result = await sut.reservePacket(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("ReservedForDay", result?.RouteValues?["Reserve"]);
            Assert.Null(result?.ControllerName);
        }

        [Fact]
        public async Task Reserve_With_Id_Of_Packet_But_Student_Cannot_Be_Found_Should_Redirect_To_List() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.ReservePacket(Arg.Any<int>(), Arg.Any<string>()).Returns("Student cannot be found");

            //Act
            var result = await sut.reservePacket(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("NotFound", result?.RouteValues?["Student"]);
            Assert.Null(result?.ControllerName);
        }

        [Fact]
        public async Task Reserve_With_Id_Of_Packet_But_Student_Is_Not_Old_Enough_Should_Redirect_To_List() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.ReservePacket(Arg.Any<int>(), Arg.Any<string>()).Returns("Student not old enough to reserve packet");

            //Act
            var result = await sut.reservePacket(-1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("NotOldEnough", result?.RouteValues?["Reserve"]);
            Assert.Null(result?.ControllerName);
        }

        [Fact]
        public async Task Reserve_With_Correct_Id_Should_Redirect_To_Detail() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.ReservePacket(Arg.Any<int>(), Arg.Any<string>()).ReturnsNull();

            //Act
            var result = await sut.reservePacket(1) as RedirectToActionResult;

            //Assert
            Assert.Equal("Detail", result?.ActionName);
            Assert.Null(result.ControllerName);
        }

        [Fact]
        public async Task Unreserve_With_Correct_Id_Should_Redirect_To_Detail() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.UnreservePacket(Arg.Any<int>(), Arg.Any<string>()).ReturnsNull();

            //Act
            var result = await sut.unreservePacket(1) as RedirectToActionResult;

            //Assert
            Assert.Equal("Detail", result?.ActionName);
            Assert.Null(result.ControllerName);
        }

        [Fact]
        public async Task Unreserve_With_Id_That_Cannot_Be_Found_Should_Redirect_To_List() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.UnreservePacket(Arg.Any<int>(), Arg.Any<string>()).Returns("Packet not found");

            //Act
            var result = await sut.unreservePacket(1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("NotFound", result?.RouteValues?["Unreserve"]);
            Assert.Null(result.ControllerName);
        }

        [Fact]
        public async Task Unreserve_With_Id_Of_Not_Reserved_Packet_Should_Redirect_To_List() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.UnreservePacket(Arg.Any<int>(), Arg.Any<string>()).Returns("Packet was not reserved");

            //Act
            var result = await sut.unreservePacket(1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("NotReserved", result?.RouteValues?["Unreserve"]);
            Assert.Null(result.ControllerName);
        }

        [Fact]
        public async Task Unreserve_With_Id_Of_Packet_That_Is_Not_Reserved_By_User_Should_Redirect_To_List() {
            //Arrange
            repoMock.GetSinglePacket(Arg.Any<int>()).Returns(new Packet() { name = "" });
            repoMock.UnreservePacket(Arg.Any<int>(), Arg.Any<string>()).Returns("Packet is not reserved by user");

            //Act
            var result = await sut.unreservePacket(1) as RedirectToActionResult;

            //Assert
            Assert.Equal("list", result?.ActionName);
            Assert.Equal("NotByUser", result?.RouteValues?["Unreserve"]);
            Assert.Null(result.ControllerName);
        }
    }
}