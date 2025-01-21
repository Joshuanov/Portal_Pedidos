using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTGU.Models;
using System.Diagnostics;

namespace SistemaTGU.Controllers
{
    [AllowAnonymous]
    public class HomeController
        (
        SignInManager<IdentityUser> signInManager,
        ILogger<HomeController> logger)
        : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Empresas");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel modelo)
        {
            var resultado = await _signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.RememberMe, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "empresas");
            }

            return View(modelo);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
