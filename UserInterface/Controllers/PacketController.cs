using Microsoft.AspNetCore.Mvc;

namespace UserInterface.Controllers {
    public class PacketController : Controller {
        public IActionResult list() {
            return View();
        }
    }
}
