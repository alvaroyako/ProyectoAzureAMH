using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetUtopia;
using ProyectoAzureAMH.Models;
using System;
using System.Collections.Generic;
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

        public ServiceApiUtopia(string urlapi)
        {
            this.UrlApi = urlapi;
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
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

        #region Metodos de Juegos

        public async Task<List<Juego>> GetJuegosAsync()
        {
            string request = "/juegos/getjuegos";
            List<Juego> juegos = await this.CallApiAsync<List<Juego>>(request);
            return juegos;
        }

        public async Task<Juego> FindJuegoAsync(int idjuego)
        {
            string request = "juegos/findjuego/" + idjuego;
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

        #region Metodos de Usuarios
        public async Task<Usuario> GetPerfilUsuarioAsync(int idusuario,string token)
        {
            string request = "/usuarios/findusuario/"+idusuario;
            Usuario usuario = await this.CallApiAsync<Usuario>(request, token);
            return usuario;
        }
        #endregion
    }
}
