using ApplicationServices;
using DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace UserInterface.Controllers {
    [Authorize]
    public class PacketController : Controller {
        private IRepository _repository;
        private IUserSession _userSession;

        public PacketController(
            IRepository repository,
            IUserSession userSession) {
            _repository = repository;
            _userSession = userSession;
        }

        [AllowAnonymous]
        public IActionResult List(int id) {
            //If user is cantine staff redidirect to CanteenContents
            
            if (_repository.UserIsCanteenStaff(_userSession.GetUserIdentityId())) {
                return RedirectToAction("CanteenContents");
            }

            var list = _repository.GetPackets();
            return View(list);
        }

        public IActionResult Reserved() {
            var list = _repository.GetReservedPackets(_userSession.GetUserIdentityId());
            return View("List", list);
        }

        [Authorize(Policy = "Staff")]
        public IActionResult CanteenContents(int id) {
            if (id == 0) {
                //TODO: get Id from User;

                var result = _repository.GetCantine(_userSession.GetUserIdentityId());
                if(result == null) return RedirectToAction("Index", "Home");
                id = result.id;
            }

            var content = _repository.GetPacketsOfCantine(id);

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

        [Authorize(Policy = "Staff")]
        [HttpPost]
        public async Task<IActionResult> Register(DomainModel.Packet packet) {    
            if(packet.startPickup >= packet.endPickup) {
                ModelState.AddModelError("CustomError", "End date can't be before start date");
            }

            if (ModelState.IsValid) {
                //set canteen connected to user
                packet.cantine = _repository.GetCantine(_userSession.GetUserIdentityId());

                //add example products
                packet.exampleProductList = _repository.GetExampleProducts(packet.typeOfMeal);

                //set 18+ attribute
                packet.SetEighteenUpValue();

                var completed = await _repository.AddPacket(packet);
                if (!completed) {
                    ModelState.AddModelError("CustomError", "Something went wrong, please try again");
                    return View();
                }
                return RedirectToAction("Index", "Home");
            } else {
                return View();
            }
        }

        [Authorize(Policy = "Staff")]
        [HttpGet]
        public IActionResult Update(int id) {
            var packet = _repository.GetSinglePacket(id);
            
            //if packet does not exist or is reserved it can not be updated
            if(packet == null) {
                return RedirectToAction("List", new { id = 0 });
            } else if(packet.reservedBy != null) {
                return RedirectToAction("List", new { id = 0 });
            }

            return View(packet);
        }

        [Authorize(Policy = "Staff")]
        [HttpPost]
        public async Task<IActionResult> Update(DomainModel.Packet packet) {
            if (packet.startPickup >= packet.endPickup) {
                ModelState.AddModelError("CustomError", "End date can't be before start date");
            }

            if (ModelState.IsValid) {
                //set cantine connected to user
                packet.cantine = _repository.GetCantine(_userSession.GetUserIdentityId());

                //add example products
                packet.exampleProductList = _repository.GetExampleProducts(packet.typeOfMeal);

                //set 18+ attribute
                packet.SetEighteenUpValue();

                var completed = await _repository.UpdatePacket(packet);
                if (!completed) {
                    ModelState.AddModelError("CustomError", "Packet ");
                    return View();
                }
                return RedirectToAction("Detail", new { id = packet.id });
            } else {
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult Detail(int id) {
            //if id == 0, than redirect to home
            if(id == 0) {
                return RedirectToAction("Index", "Home");
            }
            var packet = _repository.GetSinglePacket(id);

            //if packet == null, than packet does not exist
            if (packet == null) {
                return RedirectToAction("Index", "Home");
            }

            return View(packet);
        }

        [Authorize(Policy = "Student")]
        public async Task<IActionResult> reservePacket(int id) {          
            var packet = _repository.GetSinglePacket(id);

            //if packet == null, than packet does not exist
            if (packet == null) {
                return RedirectToAction("List", new { id = -1 });
            }
            
            //if reserve/unreserve packet is success go back to detail page, otherwise go to list page 
            var answer = await _repository.ReservePacket(id, _userSession.GetUserIdentityId());
            if(answer == null) return RedirectToAction("Detail", new { id = id });
            else return RedirectToAction("list", new { id = -1 });
        }

        [Authorize(Policy = "Student")]
        public async Task<IActionResult> unreservePacket(int id) {
            var packet = _repository.GetSinglePacket(id);

            //if packet == null, than packet does not exist
            if (packet == null) {
                return RedirectToAction("List", new { id = -2 });
            }

            //if reserve/unreserve packet is success go back to detail page, otherwise go to list page 
            var answer = await _repository.UnreservePacket(id, _userSession.GetUserIdentityId());
            if (answer == null) return RedirectToAction("Detail", new { id = id });
            else return RedirectToAction("list", new { id = -2 });
        }
    }
}