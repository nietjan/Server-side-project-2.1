﻿using ApplicationServices;
using DomainModel.enums;
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
        public IActionResult List(string? city = null, string? typeOfMeal = null) {
            //If user is Canteen staff redidirect to CanteenContents
            if (_repository.UserIsCanteenStaff(_userSession.GetUserIdentityId())) {
                return RedirectToAction("CanteenContents");
            }
            City? cityFiler = null;
            TypeOfMeal? typeOfMealFilter = null;

   
            // Turns string city into City city if string is correct
            if (city != null) {
                foreach (var item in Enum.GetValues(typeof(City))) {
                    if (item.ToString()?.ToLower() == city.ToLower()) {
                        cityFiler = (City)item;
                        break;
                    }
                }
            } else {
                //take city from student if logged in
                var userId = _userSession.GetUserIdentityId();
                if(userId != "") cityFiler = _repository.GetStudent(userId)?.studyCity;
            }

            //Turns string typeOfMeal into TypeOfMeal typeOfMeal if string is correct
            if (typeOfMeal != null) {
                foreach (var item in Enum.GetValues(typeof(TypeOfMeal))) {
                    if (item.ToString()?.ToLower() == typeOfMeal.ToLower()) {
                        typeOfMealFilter = (TypeOfMeal)item;
                        break;
                    }
                }
            }

            var list = _repository.GetPackets(cityFiler, typeOfMealFilter);
            return View(list);
        }

        public IActionResult Reserved() {
            var list = _repository.GetReservedPackets(_userSession.GetUserIdentityId());
            return View("List", list);
        }

        [Authorize(Policy = "Staff")]
        public IActionResult CanteenContents(int id) {
            if (id == 0) {
                var result = _repository.GetCanteen(_userSession.GetUserIdentityId());
                if(result == null) return RedirectToAction("Index", "Home");
                id = result.id;
            }

            var content = _repository.GetPacketsOfCanteen(id);

            //check if there is content 
            if (content == null) {
                return RedirectToAction("Index", "Home");
            }

            return View(content);
        }

        [Authorize(Policy = "Staff")]
        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [Authorize(Policy = "Staff")]
        [HttpPost]
        public async Task<IActionResult> Register(DomainModel.Packet packet) {    
            if(packet.startPickup >= packet.endPickup) {
                ModelState.AddModelError("CustomError", "End date can't be before or at the same time as start date");
            }

            if(packet.startPickup > DateTime.Now.AddDays(2)) ModelState.AddModelError("CustomError", "Start date can't be more than two days after today");
            
            if (packet.endPickup > DateTime.Now.AddDays(3)) ModelState.AddModelError("CustomError", "End date can't be more than three days after today");
            
            if (ModelState.IsValid) {
                //set canteen connected to user
                packet.canteen = _repository.GetCanteen(_userSession.GetUserIdentityId());
                packet.city = packet?.canteen?.city;

                //add example products
                packet.exampleProductList = _repository.GetExampleProducts(packet.typeOfMeal);

                //set 18+ attribute
                packet.SetEighteenUpValue();

                var completed = await _repository.AddPacket(packet);
                if (!completed) {
                    ModelState.AddModelError("CustomError", "Something went wrong, please try again");
                    return View();
                }
                return RedirectToAction("CanteenContents");
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
                return RedirectToAction("CanteenContents", new { id = 0, Update = "NotFound" });
            } else if(packet.reservedBy != null) {
                return RedirectToAction("CanteenContents", new { id = 0, Update = "Reserved" });
            }

            return View(packet);
        }

        [Authorize(Policy = "Staff")]
        [HttpPost]
        public async Task<IActionResult> Update(DomainModel.Packet packet) {
            if (packet.startPickup >= packet.endPickup) {
                ModelState.AddModelError("CustomError", "End date can't be before or at the same time as start date");
            }

            //Check if date is correct
            if (packet.startPickup > DateTime.Now.AddDays(2)) ModelState.AddModelError("CustomError", "Start date can't be more than two days after today");

            if (packet.endPickup > DateTime.Now.AddDays(3)) ModelState.AddModelError("CustomError", "End date can't be more than three days after today");

            if (ModelState.IsValid) {
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
                return View(packet);
            }
        }

        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> Delete(int id) {
            var packet = _repository.GetSinglePacket(id);

            //if packet does not exist or is reserved it can not be deleted
            if (packet == null) {
                return RedirectToAction("CanteenContents", new { id = 0, Delete = "NotFound" });
            } else if (packet.reservedBy != null) {
                return RedirectToAction("CanteenContents", new { id = 0, Delete = "Reserved" });
            }

            //delete package
            await _repository.DeletePacket(packet);

            return RedirectToAction("CanteenContents", new { id = 0 });
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
                return RedirectToAction("list", new { Reserve = "NotFound" });
            }
            
            //if reserve packet is success go back to detail page, otherwise go to list page 
            var answer = await _repository.ReservePacket(id, _userSession.GetUserIdentityId());
            if(answer == null) return RedirectToAction("Detail", new { id = id });

            switch (answer) {
                case "Packet not found":
                    return RedirectToAction("list", new { Reserve = "NotFound" });
                case "Packet already reserved":
                    return RedirectToAction("list", new { Reserve = "Reserved" });
                case "Already reserved a package":
                    return RedirectToAction("list", new { Reserve = "ReservedForDay" });
                case "Student cannot be found":
                    return RedirectToAction("list", new { Student = "NotFound" });
                case "Student not old enough to reserve packet":
                    return RedirectToAction("list", new { Reserve = "NotOldEnough" });
                default: return RedirectToAction("list");
            }
        }

        [Authorize(Policy = "Student")]
        public async Task<IActionResult> unreservePacket(int id) {
            var packet = _repository.GetSinglePacket(id);

            //if packet == null, than packet does not exist
            if (packet == null) {
                return RedirectToAction("list", new { Unreserve = "NotFound" });
            }

            //if unreserve packet is success go back to detail page, otherwise go to list page 
            var answer = await _repository.UnreservePacket(id, _userSession.GetUserIdentityId());
            if (answer == null) return RedirectToAction("Detail", new { id = id });

            switch (answer) {
                case "Packet not found":
                    return RedirectToAction("list", new { Unreserve = "NotFound" });
                case "Packet was not reserved":
                    return RedirectToAction("list", new { Unreserve = "NotReserved" });
                case "Packet is not reserved by user":
                    return RedirectToAction("list", new { Unreserve = "NotByUser" });
                default: return RedirectToAction("list");
            }
        }
    }
}