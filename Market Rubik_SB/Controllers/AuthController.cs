using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MiniMarketApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario")))
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contrasena)
        {
            // Identity usa UserName, el SignInManager.PasswordSignInAsync lo buscará por ahí
            var result = await _signInManager.PasswordSignInAsync(nombreUsuario, contrasena, isPersistent: false, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(nombreUsuario);
                var roles = await _userManager.GetRolesAsync(user!);
                var rol = roles.FirstOrDefault() ?? "Cliente";

                HttpContext.Session.SetString("Usuario", nombreUsuario);
                HttpContext.Session.SetString("NombreCompleto", nombreUsuario);
                HttpContext.Session.SetString("Rol", rol);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(string nombreUsuario, string contrasena, string nombreCompleto)
        {
            var user = new IdentityUser { UserName = nombreUsuario };
            var result = await _userManager.CreateAsync(user, contrasena);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Cliente");
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                HttpContext.Session.SetString("Usuario", nombreUsuario);
                HttpContext.Session.SetString("NombreCompleto", nombreCompleto);
                HttpContext.Session.SetString("Rol", "Cliente");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ViewBag.Error = error.Description;
                break; // Mostramos el primer error para mantener la vista simple
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
