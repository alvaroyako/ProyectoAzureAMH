using Microsoft.AspNetCore.Mvc;
using NugetUtopia;
using ProyectoAzureAMH.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoAzureAMH.Controllers
{
    public class ManageController : Controller
    {
        private ServiceApiUtopia service;

        public ManageController(ServiceApiUtopia service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            string token = await this.service.GetToken(email, password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Email/Password incorrectos";
                return View();
            }
            else
            {
                Usuario usuario = await this.service.GetPerfilUsuarioAsync(token);
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Name, empleado.Apellido));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, empleado.IdEmpleado.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, empleado.Oficio));
                identity.AddClaim(new Claim("TOKEN", token));
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });
                return RedirectToAction("Index", "Home");
            }

        }
    }
}
