using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiOAuthEmpleados.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }


        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            return await this.context.Empleados.ToListAsync();
        }
        public async Task<Empleado> FindEMpleadoByIdAsync(int id)
        {
            return await this.context.Empleados.FirstOrDefaultAsync(z=>z.IdEmpelado==id);
        }

        //PARA LA SEGURIDAD
        public async Task<Empleado> LogInEmpleado(string apellido,int idEmpleado)
        {
            return await this.context.Empleados.Where(z =>z.Apellido == apellido && z.IdEmpelado == idEmpleado).FirstOrDefaultAsync();
        }

        public async Task<List<Empleado>> GetCompisAsync(int idDepartamento)
        {
            return await this.context.Empleados.Where(z=>z.IdDepartamento==idDepartamento).ToListAsync();
        }

        //Metodos con multiples parametros 09/04/2026

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Empleados select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosPorOficiosAsync(List<string> oficios)
        {
            var consulta = from datos in this.context.Empleados where oficios.Contains(datos.Oficio) 
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task IncrementarSalarioEmpleadosPorOficiosAsync(int incremento,List<string> oficios)
        {
            List<Empleado> empleados = await this.GetEmpleadosPorOficiosAsync(oficios);
            foreach (Empleado emp  in empleados)
            {
                emp.Salario += incremento;
            }
            await this.context.SaveChangesAsync();
        }

    }
}
