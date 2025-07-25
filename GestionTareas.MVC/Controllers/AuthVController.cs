using GestionTareas.API.Consumer;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionTareas.MVC.Controllers
{
    public class AuthVController : Controller
    {
        private readonly string _baseUrl;

        public AuthVController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7292";
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await Crud<UserResponse>.Post($"{_baseUrl}/api/Auth/login", model);

                if (response != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, response.Id.ToString()),
                        new Claim(ClaimTypes.Name, response.Nombre),
                        new Claim(ClaimTypes.Email, response.Correo),
                        new Claim("JWTToken", response.Token)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ModelState.AddModelError("", "Credenciales inválidas");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await Crud<Usuario>.Post($"{_baseUrl}/api/Auth/Register", model);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al registrar el usuario: " + ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}