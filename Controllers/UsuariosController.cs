
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTGU.Entities;
using SistemaTGU.Models;

namespace SistemaTGU.Controllers
{
    public class UsuariosController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager) : Controller
    {

        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;

        public IActionResult Index()
        {
            return View();
        }


        // Registro (GET)
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        // Registro (POST)
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = new Usuario
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre
                    
                    
                };

                var resultado = await _userManager.CreateAsync(usuario, model.Password);

                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // Login (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login (POST)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Inicio de sesión no válido.");
            }
            return View(model);
        }

        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }
    }



}
