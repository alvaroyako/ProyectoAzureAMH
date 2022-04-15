﻿using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetUtopia;
using ProyectoAzureAMH.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAzureAMH.Services
{
    public class ServiceApiUtopia
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue Header;
        private BlobServiceClient client;

        public ServiceApiUtopia(string urlapi,BlobServiceClient client)
        {
            this.UrlApi = urlapi;
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.client = client;
        }

        //Este metodo no necesita el token para funcionar
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        //Este metodo necesita el token para funcionar
        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        #region Login y Register
        //Permite recuperar el token
        public async Task<string> GetToken(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    Email = email,
                    Password = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                string request = "/auth/login";
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(data);
                    string token = jObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<int> RegistrarUsuarioAsync(string nombre, string email, string password, string imagen)
        {
            int idusu = await this.GetMaxIdUsuario();
            
            using (HttpClient client = new HttpClient())
            {
                string request = "/usuarios/registrarusuario";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Usuario usu = new Usuario();
                usu.IdUsuario = 0;
                usu.Nombre = nombre;
                usu.Email = email;
                usu.Password = null;
                usu.Salt = null;
                usu.Imagen =imagen;
                usu.Rol = "cliente";
                usu.PasswordString = password;

                string json = JsonConvert.SerializeObject(usu);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
            return idusu;
        }
        #endregion

        #region Metodos de Juegos

        public async Task<List<Juego>> GetJuegosAsync()
        {
            string request = "/juegos/getjuegos";
            List<Juego> juegos = await this.CallApiAsync<List<Juego>>(request);
            return juegos;
        }

        public async Task<Juego> FindJuegoAsync(int idjuego)
        {
            string request = "/juegos/findjuego/" + idjuego;
            Juego juego = await this.CallApiAsync<Juego>(request);
            return juego;
        }

        public async Task<Juego> FindJuegoNombreAsync(string nombre)
        {
            string request = "/juegos/buscarjuegonombre/" + nombre;
            Juego juego = await this.CallApiAsync<Juego>(request);
            return juego;
        }

        public async Task CrearJuegoAsync(Juego juego,string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/juegos/crearjuego";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                Juego j = new Juego();
                j.IdJuego = juego.IdJuego;
                j.Nombre = juego.Nombre;
                j.Descripcion = juego.Descripcion;
                j.Categoria = juego.Categoria;
                j.Precio = juego.Precio;
                j.Foto = juego.Foto;

                string json = JsonConvert.SerializeObject(j);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task UpdateJuegoAsync(Juego juego,string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/juegos/updatejuego";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                Juego j = new Juego();
                j.IdJuego = juego.IdJuego;
                j.Nombre = juego.Nombre;
                j.Descripcion = juego.Descripcion;
                j.Categoria = juego.Categoria;
                j.Precio = juego.Precio;
                j.Foto = juego.Foto;

                string json = JsonConvert.SerializeObject(j);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
            }
        }

        public async Task DeleteJuegoAsync(int idjuego,string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/juegos/DeleteJuego/" + idjuego;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        #endregion

        #region Metodos de Platos
        public async Task<List<Plato>> GetPlatosAsync()
        {
            string request = "/platos/getplatos";
            List<Plato> platos = await this.CallApiAsync<List<Plato>>(request);
            return platos;
        }

        public async Task<Plato> FindPlatoAsync(int idplato)
        {
            string request = "platos/findplato/" + idplato;
            Plato plato = await this.CallApiAsync<Plato>(request);
            return plato;
        }

        public async Task CrearPlatoAsync(Plato plato, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/platos/crearplato";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                Plato p = new Plato();
                p.IdPlato = plato.IdPlato;
                p.Nombre = plato.Nombre;
                p.Descripcion = plato.Descripcion;
                p.Categoria = plato.Categoria;
                p.Precio = plato.Precio;
                p.Foto = plato.Foto;

                string json = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task UpdatePlatoAsync(Plato plato, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/platos/updateplato";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                Plato p = new Plato();
                p.IdPlato = plato.IdPlato;
                p.Nombre = plato.Nombre;
                p.Descripcion = plato.Descripcion;
                p.Categoria = plato.Categoria;
                p.Precio = plato.Precio;
                p.Foto = plato.Foto;

                string json = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
            }
        }

        public async Task DeletePlatoAsync(int idplato, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/platos/DeletePlato/" + idplato;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }
        #endregion

        #region Metodos de Reservas
        public async Task<List<Reserva>> GetReservasAsync(string token)
        {
            string request = "/reservas/getreservas";
            List<Reserva> reservas = await this.CallApiAsync<List<Reserva>>(request,token);
            return reservas;
        }

        public async Task<Reserva> FindReservaAsync(string nombre)
        {
            string request = "/reservas/findreserva/"+nombre;
            Reserva reserva= await this.CallApiAsync<Reserva>(request);
            return reserva;
        }

        public async Task CrearReservaAsync(Reserva reserva)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/Reservas/CrearReserva";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(reserva);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task DeleteReservaAsync(string nombre, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/reservas/Deletereserva/" + nombre;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task UpdateReservaAsync(Reserva reserva, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/reservas/updatereserva";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(reserva);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
            }
        }
        #endregion

        #region Metodos de Usuarios
        public async Task<Usuario> GetPerfilUsuarioAsync(string token)
        {
            string request = "/usuarios/perfilusuario";
            Usuario usuario = await this.CallApiAsync<Usuario>(request, token);
            return usuario;
        }
        #endregion

        #region Metodos de compras
        public async Task<List<Compra>> BuscarComprasAsync(int idusuario,string token)
        {
            string request = "/compras/buscarcomprasusuario/" + idusuario;
            List<Compra> compras = await this.CallApiAsync<List<Compra>>(request,token);
            return compras;
        }

        #endregion

        #region Blobs
        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }
        #endregion

        #region Otros
        private async Task <int> GetMaxIdUsuario()
        {
            string request = "/otros/getmaxidusuarios";
            int idusuario = await this.CallApiAsync<int>(request);
            return idusuario;
        }

        private async Task<int> GetMaxIdCompra()
        {
            string request = "/otros/getmaxidcompras";
            int idcompra = await this.CallApiAsync<int>(request);
            return idcompra;
        }
        #endregion


    }
}
