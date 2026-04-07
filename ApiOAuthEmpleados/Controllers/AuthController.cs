using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryHospital repo;
        //Importante el helper tmb
        private HelperActionOAuthService helper;
        public AuthController(RepositoryHospital repo, HelperActionOAuthService helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        //METODO PARA VALIDAR EL USUARIO(POST)
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            Empleado empleado = await this.repo.LogInEmpleado(model.UserName, int.Parse(model.Password));
            if (empleado == null)
            {
                return Unauthorized();
            }
            else
            {
                //DEBEMOS CREAR UNAS CREDENCIALES CON NUESTRO TOKEN
                SigningCredentials credentials = new SigningCredentials(this.helper.GetKeyToken(),SecurityAlgorithms.HmacSha256);

                //EL TOKEN SE GENERA CON UNA CLASE Y DEBEMOS ALMACENAR LOS DATOS DE ISSUER,CREDENTIALS....
                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    signingCredentials: credentials,//credenciales creadas anteriormente
                    expires:DateTime.UtcNow.AddMinutes(20),//tiempo que se expira
                    notBefore:DateTime.UtcNow//no validar antes
                    );

                //POR ULTIMO DEVOLVEMOS LA RESPUESTA AFIRMATIVA CON EL TOKEN
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
        
    }
}
