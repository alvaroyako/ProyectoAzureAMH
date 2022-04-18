﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NugetUtopia;
using ProyectoAzureAMH.Filters;
using ProyectoAzureAMH.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoAzureAMH.Controllers
{
    public class PlatosController : Controller
    {
        private ServiceApiUtopia service;

        public PlatosController(ServiceApiUtopia service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<Plato> platos = await this.service.GetPlatosAsync();
            return View(platos);
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public IActionResult CrearPlato()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CrearPlato(int idplato, string nombre, string descripcion, string categoria, int precio, IFormFile foto)
        {
            string filename = foto.FileName;
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Plato plato = new Plato();
            plato.IdPlato = idplato;
            plato.Nombre = nombre;
            plato.Descripcion = descripcion;
            plato.Categoria = categoria;
            plato.Precio = precio;
            plato.Foto = filename;
            await this.service.CrearPlatoAsync(plato,token);
        
            using (Stream stream = foto.OpenReadStream())
            {
                await this.service.UploadBlobAsync("platos", filename, stream);

            }
            return RedirectToAction("Index", "Admin");
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<ActionResult> DeletePlato(int idplato)
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Plato plato = await this.service.FindPlatoAsync(idplato);
            await this.service.DeleteBlobAsync("platos", plato.Foto);
            await this.service.DeletePlatoAsync(idplato,token);
            return RedirectToAction("Index", "Admin");
        }

        [AuthorizeUsuarios(Policy = "AdminOnly")]
        public async Task<IActionResult> EditarPlato(int idplato)
        {
            Plato plato = await this.service.FindPlatoAsync(idplato);
            return View(plato);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPlato(Plato plato, IFormFile archivo)
        {
            await this.service.DeleteBlobAsync("platos", plato.Foto);

            string filename = archivo.FileName;
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            plato.Foto = filename;
            await this.service.UpdatePlatoAsync(plato, token);

            using (Stream stream = archivo.OpenReadStream())
            {
                await this.service.UploadBlobAsync("platos", filename, stream);

            }

            return RedirectToAction("Index", "Admin");
        }

    }
}
