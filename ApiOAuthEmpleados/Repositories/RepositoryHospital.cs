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
    }
}
