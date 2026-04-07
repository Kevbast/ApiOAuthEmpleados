using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcOAuthEmpleados.Models
{
    public class Empleado
    {//clase limpia sin mapeos
        public int IdEmpelado { get; set; }

        public string Apellido { get; set; }

        public string Oficio { get; set; }

        public int Salario { get; set; }

        public int IdDepartamento { get; set; }
    }
}
