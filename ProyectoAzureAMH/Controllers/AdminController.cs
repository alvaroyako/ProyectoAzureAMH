﻿using Microsoft.AspNetCore.Mvc;
using NugetUtopia;
using ProyectoAzureAMH.Filters;
using ProyectoAzureAMH.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoAzureAMH.Controllers
{
    public class AdminController : Controller
    {
        private ServiceApiUtopia service;

        public AdminController(ServiceApiUtopia service)
        {
            this.service = service;
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> Index()
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            List<Plato> platos = await this.service.GetPlatosAsync();
            List<Juego> juegos = await this.service.GetJuegosAsync();
            List<Reserva> reservas = await this.service.GetReservasAsync(token);

            ViewData["PLATOS"] = platos;
            ViewData["JUEGOS"] = juegos;
            return View(reservas);
        }
    }
}
