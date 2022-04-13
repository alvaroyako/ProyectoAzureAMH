using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoAzureAMH.Controllers
{
    public class JuegosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
