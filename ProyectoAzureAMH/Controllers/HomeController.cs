using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NugetUtopia;
using ProyectoAzureAMH.Filters;
using ProyectoAzureAMH.Models;
using ProyectoAzureAMH.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoAzureAMH.Controllers
{
    public class HomeController : Controller
    {
        private ServiceApiUtopia service;
        public HomeController(ServiceApiUtopia service)
        {
            this.service = service;
        }

        [AuthorizeUsuarios]
        public IActionResult GoToHome()
        {

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Index()
        {
            List<Juego> juegos = await this.service.GetJuegosAsync();
            return View(juegos);
        }

        public IActionResult SobreNosotros()
        {
            return View();
        }
    }
}
