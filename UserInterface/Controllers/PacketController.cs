using DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserInterface.Controllers {
    [Authorize]
    public class PacketController : Controller {
        private IRepository repository;
        private static readonly string reservedid = "test@test.com";

        public PacketController(IRepository repository) {
            this.repository = repository;
        }

        public IActionResult List(int id) {
            //If user is cantine staff redidirect to CanteenContents
            if (false) {
                return RedirectToAction("CanteenContents");
            }

            var list = repository.GetPackets();
            return View(list);
        }

        public IActionResult Reserved() {
            var list = repository.GetReservedPackets(reservedid);
            return View("List", list);
        }

        [Authorize(Policy = "Claim.test")]
        public IActionResult CanteenContents(int id) {
            if (id == 0) {
                //TODO: get Id from User;
                id = 1;
            }

            var content = repository.GetPacketsOfCantine(id);

            //check if there is content 
            if (content == null) {
                return RedirectToAction("Index", "Home");
            }

            return View(content);
        }

        [Authorize(Policy = "canteenStaff")]
        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [Authorize(Policy = "canteenStaff")]
        [HttpPost]
        public async Task<IActionResult> Register(DomainModel.Packet packet) {    
            if(packet.startPickup >= packet.endPickup) {
                ModelState.AddModelError("CustomError", "End date can't be before start date");
            }

            if (ModelState.IsValid) {
                //set cantine connected to user
                packet.cantine = Infrastructure.InMemoryRepository.cantine;

                //add example products
                packet.exampleProductList = repository.GetExampleProducts(packet.typeOfMeal);

                var completed = await repository.AddPacket(packet);
                if (!completed) {
                    ModelState.AddModelError("CustomError", "Something went wrong, please try again");
                    return View();
                }
                return RedirectToAction("Index", "Home");
            } else {
                return View();
            }
        }

        [Authorize(Policy = "canteenStaff")]
        [HttpGet]
        public IActionResult Update(int id) {
            var packet = repository.GetSinglePacket(id);
            
            //if packet does not exist or is reserved it can not be updated
            if(packet == null) {
                return RedirectToAction("List", new { id = 0 });
            } else if(packet.reservedBy != null) {
                return RedirectToAction("List", new { id = 0 });
            }

            return View(packet);
        }

        [Authorize(Policy = "canteenStaff")]
        [HttpPost]
        public async Task<IActionResult> Update(DomainModel.Packet packet) {
            if (packet.startPickup >= packet.endPickup) {
                ModelState.AddModelError("CustomError", "End date can't be before start date");
            }

            if (ModelState.IsValid) {
                //set cantine connected to user
                packet.cantine = Infrastructure.InMemoryRepository.cantine;

                //add example products
                packet.exampleProductList = repository.GetExampleProducts(packet.typeOfMeal);

                var completed = await repository.UpdatePacket(packet);
                if (!completed) {
                    ModelState.AddModelError("CustomError", "Something went wrong, please try again");
                    return View();
                }
                return RedirectToAction("Detail", new { id = packet.id });
            } else {
                return View();
            }
        }

        public IActionResult Detail(int id) {
            //if id == 0, than redirect to home
            if(id == 0) {
                return RedirectToAction("Index", "Home");
            }
            var packet = repository.GetSinglePacket(id);

            //if packet == null, than packet does not exist
            if (packet == null) {
                return RedirectToAction("Index", "Home");
            }

            return View(packet);
        }

        public async Task<IActionResult> reservePacket(int id) {          
            var packet = repository.GetSinglePacket(id);

            //if packet == null, than packet does not exist
            if (packet == null) {
                return RedirectToAction("List", new { id = 0 });
            }
            
            //if reserve packet is succes go back to detail page, otherwise go to list page 
            //TODO: change email to email of user
            var answer = await repository.reservePacket(id, "test@test.com");
            return RedirectToAction("Detail", new { id = id });
        }
    }
}