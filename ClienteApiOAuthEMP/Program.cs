// See https://aka.ms/new-console-template for more information
using ClienteApiOAuthEMP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

Console.WriteLine("Cliente Api OAuth");
Console.WriteLine("Introduzca Apellido:");
string apellido = Console.ReadLine();
Console.WriteLine("Introduzca password:");
//PARA CREAR MÉTODOS EN PROGRAM DEBEMOS HACERLOS STATIC
string password = Console.ReadLine();

string respuesta = await GetTokenAsync(apellido, password);
Console.WriteLine(respuesta);
Console.WriteLine("--------------------------------------");

Console.WriteLine("INTRODUZCA UN ID DE EMPLEADO A BUSCAR");
string idEmpleado = Console.ReadLine();
string datos = await FindEmpleadoAsync(int.Parse(idEmpleado), respuesta);

Console.WriteLine(datos);
static async Task<string> GetTokenAsync(string user,string pass)
{
    string urlApi = "https://apioauthempleadoskbs.azurewebsites.net/";
    LoginModel model = new LoginModel
    {
        Username = user,
        Password = pass
    };
    using (HttpClient client = new HttpClient())
    {
        string request = "/api/Auth/Login";
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        string json = JsonConvert.SerializeObject(model);

        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(request, content);
        if (response.IsSuccessStatusCode==true)
        {
            string data = await response.Content.ReadAsStringAsync();
            JObject objeto = JObject.Parse(data);
            string token = objeto.GetValue("response").ToString();
            
            return token;
        }
        else
        {
            return "PETICION INCORRECTA: " + response.RequestMessage;
        }
    } ;
}

static async Task<string> FindEmpleadoAsync(int idEmpleado,string token)
{
    //misma url
    string urlApi = "https://apioauthempleadoskbs.azurewebsites.net/";
    using (HttpClient client = new HttpClient())
    {
        string request = "/api/Empleados/" + idEmpleado;
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //LO UNICO DE NOVEDAD ES INCLUIR authorization con bearer dentro de header
        client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

        HttpResponseMessage response = await client.GetAsync(request);

        if (response.IsSuccessStatusCode == true)
        {
            string data = await response.Content.ReadAsStringAsync();
                return data;
        }
        else
        {
            return "Error en la petición: " + response.StatusCode;
        }
    }
}
