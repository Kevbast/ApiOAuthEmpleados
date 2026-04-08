using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Filters;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;
using System.Security.Claims;
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
        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }
        [AuthorizeEmpleados]
        public async Task<IActionResult> Details(int idEmpleado)
        {

                Empleado empleado = await this.service.FindEmpleadoById(idEmpleado);
                return View(empleado);
            
        }

        [AuthorizeEmpleados]//VAMOS A CAMBIARLO
        public async Task<IActionResult> PerfilEmpleado()
        {
                Empleado empleado = await this.service.GetPerfilAsync();
                return View(empleado);
            
        }
        [AuthorizeEmpleados]
        public async Task<IActionResult> Compis()
        {
            List<Empleado> empleados = await this.service.GetCompisAsync();
            return View(empleados);
        }




    }
}
