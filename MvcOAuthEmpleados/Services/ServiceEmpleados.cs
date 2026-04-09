using MvcOAuthEmpleados.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.JavaScript;
using System.Text;

namespace MvcOAuthEmpleados.Services
{
    public class ServiceEmpleados
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;

        //INYECTAMOS IHTTPCONTEXTACCESOR //SE INYECTA TMB EN PROGRAM
        private IHttpContextAccessor contextAccessor;
        public ServiceEmpleados(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiEmpleados");
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            //para poder acceder a los claims del usuario y también su token
            this.contextAccessor = contextAccessor;
        }

        //TANTO EN ICREMENTAR COMO EN BUSCAR LOS EMPLEADOS POR OFICIOS NECESITAMOS GENERAR EL SIGUIENTE STRING PARA EL REQUEST
        //oficio=ANALISTA&oficio=EMPLEADO

        private string TransformCollectionToQuery(List<string> collection)
        {
            string result = "";
            foreach (string oficio  in collection)
            {
                result += "oficio=" + oficio + "&";
            }
            result = result.TrimEnd('&');
            return result;
        }
        public async Task<List<Empleado>> GetEmpleadoOficiosAsync(List<string> oficios)
        {
            string request = "api/Empleados/EmpleadosOficios";
            string data = this.TransformCollectionToQuery(oficios);
            List<Empleado> empleados = await this.CallApiAsync<List<Empleado>>(request + "?" + data);
            return empleados;
        }
        //PRIMER UPDATE
        public async Task IncrementarSalarioEmpOficiosAsync(int incremento,List<string> oficios)
        {
            string request = "api/Empleados/IncrementarSalariosEmpOficio/"+incremento;
            string data = this.TransformCollectionToQuery(oficios);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response = await client.PutAsync(request + "?"+ data, null);
            }
        }
        public async Task<List<string>> GetOficiosAsync()
        {
            string request = "api/Empleados/Oficios";
            List<string> oficios = await this.CallApiAsync<List<string>>(request);
            return oficios;
        }

        //======

        public async Task<string> LoginAsync(string user,string pass)//es el GetTokenAsync
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Auth/Login";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                LoginModel model = new LoginModel
                {
                    UserName = user,
                    Password = pass
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, this.header);

                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode == true)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }
        //MÉTODOS PRIVADOS
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode == true)
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

        //REALIZAMOS UNA SORECARGA
        private async Task<T> CallApiAsync<T>(string request,string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode == true)
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

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            string request = "/api/Empleados";
            List<Empleado> empleados = await this.CallApiAsync<List<Empleado>>(request);
            return empleados;
        }
        public async Task<Empleado> FindEmpleadoById(int idEmpleado)//el token está en el claim!!
        {
            string token = contextAccessor.HttpContext.User.FindFirst(z=>z.Type=="TOKEN").Value;
            //LO RECUPERAMOS MANUALMENTE
            string request = "/api/Empleados/"+idEmpleado;
            Empleado empleado = await CallApiAsync<Empleado>(request, token);

            return empleado;
        }

        //NUEVOS MÉTODOS IMPLEMENTADOS EN LA API
        public async Task<Empleado> GetPerfilAsync( )//el token está en el claim!!
        {
            string token = contextAccessor.HttpContext.User.FindFirst(z=>z.Type=="TOKEN").Value;
            //LO RECUPERAMOS MANUALMENTE
            string request = "/api/Empleados/Perfil";
            Empleado empleado = await CallApiAsync<Empleado>(request, token);

            return empleado;
        }

        public async Task<List<Empleado>> GetCompisAsync()
        {
            string token = contextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            string request = "/api/Empleados/Compis";
            List<Empleado> empleados = await this.CallApiAsync<List<Empleado>>(request,token);
            return empleados;
        }



    }
}
