using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        public RepositoryHospital repo;
        private HelperActionOAuthService helperOAuth;
        private HelperCifrado helperCifrado;

        public EmpleadosController(RepositoryHospital repo, HelperActionOAuthService helperOAuth, HelperCifrado helperCifrado)
        {
            this.repo = repo;
            this.helperOAuth = helperOAuth;
            this.helperCifrado = helperCifrado;
        }

        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> GetEmpleados()
        {
            return await this.repo.GetEmpleadosAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> FindEmpleado(int id)
        {
            return await this.repo.FindEMpleadoByIdAsync(id);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]//no puede tener el mismo route que los otros q devuelven lo mismo
        public async Task<ActionResult<Empleado>> Perfil()
        {
            //COMO ESTÁ AUTORIZADO HAY TOKEN,POR LO TANTO TENDRÍAMOS CLAIM
            Claim claim = HttpContext.User.FindFirst(x => x.Type == "UserData");
            if (claim is null)
            {
                return Unauthorized();
            }

            try
            {
                Empleado empleado = this.GetEmpleadoFromEncryptedClaim(claim.Value);
                return await this.repo.FindEMpleadoByIdAsync(empleado.IdEmpelado);
            }
            catch (FormatException)
            {
                return Unauthorized();
            }
            catch (CryptographicException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return Unauthorized();
            }
        }
        [Authorize(Roles ="PRESIDENTE")]//LO AÑADIMOS LA RESTRICCION EN AUTHORIZE
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            //COMO ESTÁ AUTORIZADO HAY TOKEN,POR LO TANTO TENDRÍAMOS CLAIM
            Claim claim = HttpContext.User.FindFirst(x => x.Type == "UserData");
            if (claim is null)
            {
                return Unauthorized();
            }

            try
            {
                Empleado empleado = this.GetEmpleadoFromEncryptedClaim(claim.Value);
                return await this.repo.GetCompisAsync(empleado.IdDepartamento);
            }
            catch (FormatException)
            {
                return Unauthorized();
            }
            catch (CryptographicException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return Unauthorized();
            }
        }

        private Empleado GetEmpleadoFromEncryptedClaim(string encryptedUserData)
        {
            ArgumentNullException.ThrowIfNull(encryptedUserData);
            if (string.IsNullOrWhiteSpace(encryptedUserData))
            {
                throw new ArgumentException("El claim cifrado no puede estar vacío.", nameof(encryptedUserData));
            }

            byte[] keyData = this.helperOAuth.GetDataEncryptionKeyBytes();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedUserData);
            return this.helperCifrado.Descifrar<Empleado>(encryptedBytes, keyData);
        }

        //OFICIOS Y MULTIPLES PARAMETROS
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> Oficios()
        {
            return await this.repo.GetOficiosAsync();
        }
        //?oficio=ANALISTA&oficio=EMPLEADO
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> EmpleadosOficios([FromQuery] List<string> oficio)
        {

            return await this.repo.GetEmpleadosPorOficiosAsync(oficio);
        }
        [HttpPut]
        [Route("[action]/{incremento}")]//LOS FROMQUERY NO SE DECLARAN EN LOS ROUTE!!!
        public async Task<ActionResult>IncrementarSalariosEmpOficio(int incremento,[FromQuery]List<string> oficio)
        {
            await this.repo.IncrementarSalarioEmpleadosPorOficiosAsync(incremento, oficio);
            return Ok();
        }



    }
}
