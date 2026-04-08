using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcOAuthEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceEmpleados service;

        public ManagedController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginModel model)
        {
            string token =await this.service.LoginAsync(model.UserName, model.Password);

            if (token == null)
            {
                ViewData["MENSAJE"] = "USUARO O CREDENCIALES INCORRECTAS";

            }
            else{

                ViewData["MENSAJE"] = "YA TIENES TU TOKEN";
                //HttpContext.Session.SetString("TOKEN", token);//lo guardamos x ahora en session,lo añadimos en el claim,es más funcional


                ClaimsIdentity identity = new ClaimsIdentity
                    (CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                //almacenamos el nombre del usuario(bonito)
                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
                //almacenamos el password como identifier
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, model.Password));
                //AÑADIMOS EL TOKEN EN UN NUEVO CLAIM en vez de guardarlo en session
                identity.AddClaim(new Claim("TOKEN", token));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                //DAMOS DE ALTA EL USUARIO DURANTE 20 MINUTOS(COMO EN LA API)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                });
                return RedirectToAction("Index", "Empleados");

            }

            return View();
        }

        public async Task<IActionResult> CerrarSesion()
        {
            //eliminamos el claim
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }


    }
}
