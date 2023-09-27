using DomainServices;
using Microsoft.AspNetCore.Mvc;

namespace UserInterface.Controllers {
    public class PacketController : Controller {
        private IRepository repository;
        private static readonly string reservedEmail = "test@test.com";

        public PacketController(IRepository repository) {
            this.repository = repository;
        }

        public IActionResult List() {
            var list = repository.GetPackets();
            return View(list);
        }

        public IActionResult Reserved() {
            var list = repository.GetReservedPackets(reservedEmail);
            return View("List", list);
        }
    }
}