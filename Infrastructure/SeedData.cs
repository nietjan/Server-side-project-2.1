using DomainModel;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure
{
    public class SeedData {
        private readonly string password = "Secret";
        private readonly PacketContext _dbContext;
        private readonly SecurityContext _securityContext;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public SeedData(PacketContext dbContext, SecurityContext securityContext, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) {
            _dbContext = dbContext;
            _securityContext = securityContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public void SeedDatabase() {
            //migrate all dbs
            _securityContext.Database.Migrate();
            _dbContext.Database.Migrate();


            if (_userManager.Users.Count() == 0) {
                var staff = seedSecurityContextStaff().Result;
                var students = seedSecurityContextStudents().Result;
                seedDbContext(staff,students).Wait();
            }
        }

        public async Task seedDbContext(string[] staffId, string[] studentId) {
    
            Canteen[] canteens = {
                    new Canteen() {location = "Hogeschoollaan", city = DomainModel.enums.City.Breda, servesHotMeals = true },
                    new Canteen() {location = "FifthLa", city = DomainModel.enums.City.Breda, servesHotMeals = true }, 
            };

            CanteenStaffMember[] staff = { //Add id of security
                    new CanteenStaffMember() {securityId= staffId[0], canteen = canteens[0], name = "Staff1-Hogeschoolaan", staffNumber = 123 },
                    new CanteenStaffMember() {securityId=staffId[1], canteen = canteens[1], name = "Staff2-FifhtLa", staffNumber = 234 },
            };

            Product[] productsList = {
                    new Product() {alcoholic=false, name="bread1" },
                    new Product() {alcoholic=false, name="bread2" },
                    new Product() {alcoholic=false, name="bread3"},

                    new Product() {alcoholic=false, name="diner1"},
                    new Product() {alcoholic=false, name="diner2"},
                    new Product() {alcoholic=false, name="diner3"},

                    new Product() {alcoholic=true, name="drink1"},
                    new Product() {alcoholic=false, name="drink2"},
                    new Product() {alcoholic=true, name="drink3"},
            };

            ExampleProductList[] exampleList = {
                    new ExampleProductList() {list = new List<Product>(){ productsList[0], productsList[1], productsList[2]}, type = DomainModel.enums.TypeOfMeal.Bread},
                    new ExampleProductList() {list = new List<Product>(){ productsList[3], productsList[4], productsList[5]}, type = DomainModel.enums.TypeOfMeal.Diner},
                    new ExampleProductList() { list = new List<Product>(){ productsList[6], productsList[7], productsList[8]}, type = DomainModel.enums.TypeOfMeal.Drink},
            };

            Student[] students = {
                new Student(){securityId=studentId[0], name="Student1", birthday = new DateTime(2006, 1, 1), studentNumber=123, studyCity=DomainModel.enums.City.Breda},
                new Student(){securityId=studentId[1], name="Student2", birthday =new DateTime(2004, 1, 1), studentNumber=234, studyCity=DomainModel.enums.City.Breda},
            };

            Packet[] packets = {
                new Packet(){name = "packet1", canteen=canteens[0], city = canteens[0].city, exampleProductList = exampleList[0], price=5, typeOfMeal = exampleList[0].type, startPickup=DateTime.Now.AddDays(10), endPickup=DateTime.Now.AddDays(11)},
                new Packet(){name = "packet2", reservedBy = students[0], canteen=canteens[1], city = canteens[1].city, exampleProductList = exampleList[1], price=15, typeOfMeal = exampleList[1].type, startPickup=DateTime.Now.AddDays(11), endPickup=DateTime.Now.AddDays(11).AddHours(5)},
                new Packet(){name = "packet3", canteen=canteens[0], city = canteens[0].city, exampleProductList = exampleList[2], price=15, typeOfMeal = exampleList[0].type, startPickup=DateTime.Now.AddDays(10), endPickup=DateTime.Now.AddDays(11)},
                new Packet(){name = "packet4", reservedBy = students[1], canteen=canteens[0], city = canteens[1].city, exampleProductList = exampleList[0], price=2, typeOfMeal = exampleList[1].type, startPickup=DateTime.Now.AddDays(11), endPickup=DateTime.Now.AddDays(11).AddHours(5)},
                new Packet(){name = "packet5", canteen=canteens[0], city = canteens[0].city, exampleProductList = exampleList[1], price=25, typeOfMeal = exampleList[0].type, startPickup=DateTime.Now.AddDays(10), endPickup=DateTime.Now.AddDays(11)},
                new Packet(){name = "packet6", reservedBy = students[1], canteen=canteens[0], city = canteens[1].city, exampleProductList = exampleList[2], price=22, typeOfMeal = exampleList[1].type, startPickup=DateTime.Now.AddDays(11), endPickup=DateTime.Now.AddDays(11).AddHours(5)},
                new Packet(){name = "packet7", canteen=canteens[0], city = canteens[1].city, exampleProductList = exampleList[0], price=35, typeOfMeal = exampleList[0].type, startPickup=DateTime.Now.AddDays(10), endPickup=DateTime.Now.AddDays(11)},
                new Packet(){name = "packet8", reservedBy = students[1], canteen=canteens[1], city = canteens[1].city, exampleProductList = exampleList[1], price=25, typeOfMeal = exampleList[1].type, startPickup=DateTime.Now.AddDays(11), endPickup=DateTime.Now.AddDays(11).AddHours(5)},
                new Packet(){name = "packet9", canteen=canteens[0], city = canteens[1].city, exampleProductList = exampleList[2], price=5, typeOfMeal = exampleList[0].type, startPickup=DateTime.Now.AddDays(10), endPickup=DateTime.Now.AddDays(11)},
                new Packet(){name = "packet10", reservedBy = students[0], canteen=canteens[1], city = canteens[1].city, exampleProductList = exampleList[0], price=7, typeOfMeal = exampleList[1].type, startPickup=DateTime.Now.AddDays(11), endPickup=DateTime.Now.AddDays(11).AddHours(5)},
                new Packet(){name = "packet11", canteen=canteens[0], city = canteens[1].city, exampleProductList = exampleList[1], price=7, typeOfMeal = exampleList[0].type, startPickup=DateTime.Now.AddDays(10), endPickup=DateTime.Now.AddDays(11)},
                new Packet(){name = "packet12", canteen=canteens[1], city = canteens[1].city, exampleProductList = exampleList[2], price=44, typeOfMeal = exampleList[1].type, startPickup=DateTime.Now.AddDays(11), endPickup=DateTime.Now.AddDays(11).AddHours(5)},
            };

            //set 18 value for all packets
            foreach (var packet in packets) packet.SetEighteenUpValue();


            _dbContext.canteen.AddRange(canteens);
            _dbContext.canteenStaffMembers.AddRange(staff);
            _dbContext.products.AddRange(productsList);
            _dbContext.exampleProductLists.AddRange(exampleList);
            _dbContext.students.AddRange(students);
            _dbContext.packets.AddRange(packets);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string[]> seedSecurityContextStaff() {
            IdentityUser[] staff = new IdentityUser[2];
            var user1 = new IdentityUser {
                UserName = "staff1",
                Email = "staff1@avans.nl",
                PhoneNumber = "1234567890",

            };

            var user2 = new IdentityUser {
                UserName = "staff2",
                Email = "staff2@avans.nl",
                PhoneNumber = "2345678901",
            };

            

            var result = await _userManager.CreateAsync(user1, password);
            if (result.Succeeded) {
                var res = await _userManager.AddClaimAsync(user1, new Claim("userType", "canteenStaff"));
                //only create user when claim succeeds otherwise delete user again
                if (!res.Succeeded) {
                    await _userManager.DeleteAsync(user1);
                    throw new Exception($"Claim not successful added to staff1, Errors {res.Errors}");
                    
                }
            } else {
                var message = string.Join(", ", result.Errors.Select(x => "Code errors: " + x.Code + ", Description errors: " + x.Description));
                throw new Exception($"staff1 not successful added, {message}");
            }

            var result2 = await _userManager.CreateAsync(user2, password);
            if (result2.Succeeded) {
                var res = await _userManager.AddClaimAsync(user2, new Claim("userType", "canteenStaff"));
                //only create user when claim succeeds otherwise delete user again
                if (!res.Succeeded) {
                    await _userManager.DeleteAsync(user2);
                    await _userManager.DeleteAsync(user1);
                    throw new Exception($"Claim not successful added to staff1, Errors {res.Errors}");

                }
            } else {
                await _userManager.DeleteAsync(user1);
                var message = string.Join(", ", result2.Errors.Select(x => "Code errors: " + x.Code + ", Description errors: " + x.Description));
                throw new Exception($"staff1 not successful added, {message}");
            }

            string[] staffId = new string[2];
            var staff1 = await _userManager.FindByEmailAsync(user1.Email);
            staffId[0] = staff1.Id;
            var staff2 = await _userManager.FindByEmailAsync(user2.Email);
            staffId[1] = staff2.Id;

            return staffId;
        }

        public async Task<string[]> seedSecurityContextStudents() {
            IdentityUser[] students = new IdentityUser[2];
            var user1 = new IdentityUser {
                UserName = "student1",
                Email = "student1@avans.nl",
                PhoneNumber = "3456789012",
            };

            var user2 = new IdentityUser {
                UserName = "student2",
                Email = "student2@avans.nl",
                PhoneNumber = "4567890123",
            };

            var result = await _userManager.CreateAsync(user1, password);
            if (result.Succeeded) {
                var res = await _userManager.AddClaimAsync(user1, new Claim("userType", "student"));
                //only create user when claim succeeds otherwise delete user again
                if (!res.Succeeded) {
                    await _userManager.DeleteAsync(user1);

                    throw new Exception($"Claim not successful added to student1, Errors: {res.Errors}");

                }
            } else {
                var message = string.Join(", ", result.Errors.Select(x => "Code errors: " + x.Code + ", Description errors: " + x.Description));
                throw new Exception($"Claim not successful added to student1, {message}");
            }

            var result2 = await _userManager.CreateAsync(user2, password);
            if (result2.Succeeded) {
                var res = await _userManager.AddClaimAsync(user2, new Claim("userType", "student"));
                //only create user when claim succeeds otherwise delete user again
                if (!res.Succeeded) {
                    await _userManager.DeleteAsync(user2);
                    await _userManager.DeleteAsync(user1);
                    throw new Exception($"Claim not successful added to student2, Errors {res.Errors}");

                }
            } else {
                await _userManager.DeleteAsync(user1);
                var message = string.Join(", ", result2.Errors.Select(x => "Code errors: " + x.Code + ", Description errors: " + x.Description));
                throw new Exception($"student2 not successful added, {message}");
            }

            string[] studentId = new string[2];
            var student1 = await _userManager.FindByEmailAsync(user1.Email);
            studentId[0] = student1.Id;
            var student2 = await _userManager.FindByEmailAsync(user2.Email);
            studentId[1] = student2.Id;


            return studentId;
        }
    }

}
