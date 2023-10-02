using DomainServices;
using Microsoft.AspNetCore.Mvc;

namespace UserInterface.Controllers {
    public class PacketController : Controller {
        private IRepository repository;
        private static readonly string reservedEmail = "test@test.com";

        public PacketController(IRepository repository) {
            this.repository = repository;
        }

        public IActionResult List(int id) {
            //If user is cantine staff redidirect to CanteenContents
            if (true) {
                return RedirectToAction("CanteenContents");
            }

            var list = repository.GetPackets();
            return View(list);
        }

        public IActionResult Reserved() {
            var list = repository.GetReservedPackets(reservedEmail);
            return View("List", list);
        }

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

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(DomainModel.Packet packet) {    
            if(packet.startPickup >= packet.endPickup) {
                ModelState.AddModelError("CustomError", "End date can't be before start date");
            }

            if (ModelState.IsValid) {
                //set cantine connected to user
                packet.cantine = Infrastructure.InMemoryRepository.cantine;

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
    }
}