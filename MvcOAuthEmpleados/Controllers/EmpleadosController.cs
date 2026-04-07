using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;
using System.Threading.Tasks;

namespace MvcOAuthEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;
        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> Details(int idEmpleado)
        {
            //TENDREMOS EL TOKEN EN SESSION CUANDO SE VALIDE
            string token = HttpContext.Session.GetString("TOKEN");
            if (token == null)
            {
                ViewData["MENSAJE"] = "TIENES QUE LOGEARTE";
                return View();
            }
            else
            {
                Empleado empleado = await this.service.FindEmpleadoById(idEmpleado,token);
                return View(empleado);
            }
        }


    }
}
