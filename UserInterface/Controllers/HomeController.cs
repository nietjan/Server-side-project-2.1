using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace UserInterface.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(
            ILogger<HomeController> logger, 
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager) {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password) {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null) {
                //Sign in
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (signInResult.Succeeded) {
                    return RedirectToAction("Index");
                } else ModelState.AddModelError("CustomError", "Password is incorrect");
            } else ModelState.AddModelError("CustomError", "User cannot be found");

            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        public IActionResult Error() {
            return Redirect("index");
        }
    }
}