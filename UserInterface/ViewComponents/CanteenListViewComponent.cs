using ApplicationServices;
using DomainServices;
using Microsoft.AspNetCore.Mvc;

namespace UserInterface.ViewComponents {
    public class CanteenListViewComponent : ViewComponent {
        readonly IRepository _repository;
        private readonly IUserSession _userSession;

        public CanteenListViewComponent(
            IRepository repo,
            IUserSession userSession) {
            _repository = repo;
            _userSession = userSession;
        }

        public IViewComponentResult Invoke() {
            // logic for preparing data
            var Canteens = _repository.GetCanteens(_userSession.GetUserIdentityId());

            return View(Canteens);
        }
    }
}
