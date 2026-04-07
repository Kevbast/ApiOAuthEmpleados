using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiOAuthEmpleados.Helpers
{
    public class HelperActionOAuthService
    {//LA CLASE ENCARGADA DE GENERAR EL TOKEN DE
        /*
         "Issuer": "https://localhost:7747",
    "Audience": "ApiOAuthEmpleadosKevin",
    "SecretKey": "Clavecita Jejeje #$#"
         */
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        
        //en el constructor recibimos configuration
        public HelperActionOAuthService(IConfiguration configuration)
        {
            this.Issuer = configuration.GetValue<string>("ApiOAuthToken:Issuer");
            this.Audience = configuration.GetValue<string>("ApiOAuthToken:Audience");
            this.SecretKey = configuration.GetValue<string>("ApiOAuthToken:SecretKey");
        }
        //NECESITAMOS UN MÉTODO PARA GENERAR UN TOKEN A PARTIR DE NUESTRO SECRET KEY
        public SymmetricSecurityKey GetKeyToken()
        {
            //CONVERTIMOS A Byte nuestra secret key
            byte[] data = Encoding.UTF8.GetBytes(this.SecretKey);
            return new SymmetricSecurityKey(data);
        }

        //UTILIZAMOS CLASES ACTION PARA SEPARA LA CAPA DE LOS SERVICES DE AUTORIZACION DEL PROGRAM
        public Action<JwtBearerOptions> GetJWtBearerOptions()
        {
            Action<JwtBearerOptions> options = new Action<JwtBearerOptions>(options =>
            {
                //Indicamos lo que se va a validar dentro del Token para permitir el acceso
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer=true,
                    ValidateAudience=true,
                    //para validar el token por tiempo
                    ValidateLifetime=true,
                    ValidateIssuerSigningKey=true,
                    ValidIssuer=this.Issuer,
                    ValidAudience=this.Audience,
                    IssuerSigningKey=this.GetKeyToken(),
                };
            });
            //devolvemos las options
            return options;
        }

        //EL ESQUEMA DE NUESTRA VALIDACION DE JwtBearerDefaults

        public Action<AuthenticationOptions> GetAuthenticationSchema()
        {
            Action<AuthenticationOptions> options = new Action<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            return options;
        }


    }
}
