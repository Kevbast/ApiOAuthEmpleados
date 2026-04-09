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

        //EMPEZAMOS CON EL MANEJO DE MULTIPLES OFICIOS

        public async Task<IActionResult> EmpleadosOficios()
        {
            List<string> oficios = await this.service.GetOficiosAsync();
            ViewData["OFICIOS"] = oficios;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficios(int? incremento,string accion,List<string> oficio)//oficio!
        {
            List<string> oficios = await this.service.GetOficiosAsync();
            ViewData["OFICIOS"] = oficios;
            if(accion.ToLower()=="update")
            {
                await this.service.IncrementarSalarioEmpOficiosAsync(incremento.Value, oficio);//SOLO A LOS OFICIOS PASADOS
            }
            List<Empleado> empleados = await this.service.GetEmpleadoOficiosAsync(oficio);
            return View(empleados);
        }


    }
}
