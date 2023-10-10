using DomainModel;
using DomainModel.enums;

namespace DomainModelTests {
    public class PacketTests {
        //SetEighteenUpValue
        [Fact]
        public void Set_Eighteen_Up_Value_Sets_False_If_Example_Product_List_Equals_Null() {
            //Arrange
            Packet packet = new Packet() { name = "Test" };

            //Act
            packet.SetEighteenUpValue();

            //Assert
            Assert.False(packet.eighteenUp);
        }

        [Fact]
        public void Set_Eighteen_Up_Value_Sets_False_If_All_Products_In_Example_Product_List_Are_Not_Eighteen_Up() {
            //Arrange
            var list = new ExampleProductList() {
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
            };
            Packet packet = new Packet() { name = "Test", exampleProductList = list };

            //Act
            packet.SetEighteenUpValue();

            //Assert
            Assert.False(packet.eighteenUp);
        }

        [Fact]
        public void Set_Eighteen_Up_Value_Sets_True_If_All_Products_In_Example_Product_List_Are_Eighteen_Up() {
            //Arrange
            var list = new ExampleProductList() {
                id = 1,
                list = new List<Product>() {
                    new Product() {
                        id = 1,
                        name = "Bread1",
                        alcoholic = true,
                    }, new Product() {
                        id = 2,
                        name = "Bread2",
                        alcoholic = true,
                    }, new Product() {
                        id = 3,
                        name = "Bread3",
                        alcoholic = true,
                    }
                },
                type = TypeOfMeal.Bread
            };
            Packet packet = new Packet() { name = "Test", exampleProductList = list };

            //Act
            packet.SetEighteenUpValue();

            //Assert
            Assert.True(packet.eighteenUp);
        }

        public void Set_Eighteen_Up_Value_Sets_True_If_One_Product_In_Example_Product_List_Is_Eighteen_Up() {
            //Arrange
            var list = new ExampleProductList() {
                id = 1,
                list = new List<Product>() {
                    new Product() {
                        id = 1,
                        name = "Bread1",
                        alcoholic = true,
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
            };
            Packet packet = new Packet() { name = "Test", exampleProductList = list };

            //Act
            packet.SetEighteenUpValue();

            //Assert
            Assert.False(packet.eighteenUp);
        }


        //StudentIsAllowedToReservePacketByAge
        [Fact]
        public void Student_Is_Allowed_To_Reserve_Packet_By_Age_Equals_False_When_Student_Is_Null() {
            //Arrange
            Packet packet = new Packet() { name = "Test" };

            //Act
            var result = packet.StudentIsAllowedToReservePacketByAge(null);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Student_Is_Allowed_To_Reserve_Packet_By_Age_Equals_True_When_Student_Is_Older_Than_Eighteen() {
            //Arrange
            Packet packet = new Packet() { name = "Test" };

            //Act
            var result = packet.StudentIsAllowedToReservePacketByAge(new Student() { name = "", studentNumber=123, studyCity=City.Breda, birthday = new DateTime(2005, 1, 1)});

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Student_Is_Allowed_To_Reserve_Packet_By_Age_Equals_False_When_Student_Is_Younger_Than_Eighteen_And_Packet_Is_Eighteen_Up() {
            //Arrange
            Packet packet = new Packet() { name = "Test", eighteenUp = true };

            //Act
            var result = packet.StudentIsAllowedToReservePacketByAge(new Student() { name = "", studentNumber = 123, studyCity = City.Breda, birthday = new DateTime(2007, 1, 1) });

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Student_Is_Allowed_To_Reserve_Packet_By_Age_Equals_True_When_Student_Is_Younger_Than_Eighteen_And_Packet_Is_Not_Eighteen_Up() {
            //Arrange
            Packet packet = new Packet() { name = "Test", eighteenUp = false };

            //Act
            var result = packet.StudentIsAllowedToReservePacketByAge(new Student() { name = "", studentNumber = 123, studyCity = City.Breda, birthday = new DateTime(2007, 1, 1) });

            //Assert
            Assert.True(result);
        }

    }
}