using Microsoft.AspNetCore.Http;
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
    public class JuegosController : Controller
    {
        private ServiceApiUtopia service;

        public JuegosController(ServiceApiUtopia service)
        {
            this.service = service;
        }

        public async Task <IActionResult> Index()
        {
            List<Juego> juegos = await this.service.GetJuegosAsync();
            return View(juegos);
        }

        [AuthorizeUsuarios]
        public IActionResult CrearJuego()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CrearJuego(int idjuego, string nombre, string descripcion, string categoria, int precio, IFormFile foto)
        {
            string filename = foto.FileName;
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Juego juego= new Juego();
            juego.IdJuego = idjuego;
            juego.Nombre = nombre;
            juego.Descripcion = descripcion;
            juego.Categoria = categoria;
            juego.Precio = precio;
            juego.Foto = filename;
            await this.service.CrearJuegoAsync(juego, token);

            using (Stream stream = foto.OpenReadStream())
            {
                await this.service.UploadBlobAsync("juegos", filename, stream);

            }
            return RedirectToAction("Index", "Admin");
        }

        [AuthorizeUsuarios]
        public async Task<ActionResult> DeleteJuego(int idjuego)
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Juego juego = await this.service.FindJuegoAsync(idjuego);
            await this.service.DeleteBlobAsync("juegos", juego.Foto);
            await this.service.DeleteJuegoAsync(idjuego, token);
            return RedirectToAction("Index", "Admin");
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> EditarJuego(int idjuego)
        {
            Juego juego = await this.service.FindJuegoAsync(idjuego);
            return View(juego);
        }

        [HttpPost]
        public async Task<IActionResult> EditarJuego(Juego juego, IFormFile archivo)
        {
            await this.service.DeleteBlobAsync("juegos", juego.Foto);

            string filename = archivo.FileName;
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            juego.Foto = filename;
            await this.service.UpdateJuegoAsync(juego, token);

            using (Stream stream = archivo.OpenReadStream())
            {
                await this.service.UploadBlobAsync("juegos", filename, stream);

            }

            return RedirectToAction("Index", "Admin");
        }

        [AuthorizeUsuarios]
        public IActionResult Favoritos()
        {
            List<Juego> favoritos =
                this.service.GetFavorito();
            if (favoritos == null)
            {
                return View();
            }
            else
            {
                return View(favoritos);
            }
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> SeleccionarFavorito(int idjuego)
        {           
            Juego favorito = await this.service.FindJuegoAsync(idjuego);
            this.service.AddFavorito(favorito);
            return RedirectToAction("Favoritos");
        }

        [AuthorizeUsuarios]
        public IActionResult EliminarFavorito(int idjuego)
        {
            this.service.DeleteFavorito(idjuego);
            return RedirectToAction("Favoritos");
        }

    }
}
