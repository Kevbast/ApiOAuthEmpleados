using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MvcOAuthEmpleados.Controllers
{
    public class HomeController : Controller
    {
        private SecretClient secretClient;

        public HomeController(SecretClient secretClient)
        {
            this.secretClient = secretClient;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string secretname)
        {
            KeyVaultSecret secret=  await secretClient.GetSecretAsync(secretname);
            ViewData["SECRETO"] = secret.Value; 
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
