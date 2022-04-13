using Microsoft.AspNetCore.Mvc;
using NugetUtopia;
using ProyectoAzureAMH.Filters;
using ProyectoAzureAMH.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoAzureAMH.Controllers
{
    public class UsuariosController : Controller
    {
        private ServiceApiUtopia service;
        public UsuariosController(ServiceApiUtopia service)
        {
            this.service = service;
        }


        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> Perfil()
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Usuario usuario =
                await this.service.GetPerfilUsuarioAsync(token);
            return View(usuario);
        }

    }
}
