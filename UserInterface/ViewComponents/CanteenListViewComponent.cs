using DomainServices;
using Microsoft.AspNetCore.Mvc;

namespace UserInterface.ViewComponents {
    public class CanteenListViewComponent : ViewComponent {
        readonly IRepository repository;
        public CanteenListViewComponent(IRepository repo) {
            repository = repo;
        }

        public IViewComponentResult Invoke() {
            // logic for preparing data
            var canteens = repository.GetCantines(1);

            return View(canteens);
        }
    }
}
