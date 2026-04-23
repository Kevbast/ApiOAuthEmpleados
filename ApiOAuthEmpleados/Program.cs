using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Repositories;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Scalar.AspNetCore;
using System;

var builder = WebApplication.CreateBuilder(args);
//IMPLEMENTAMOS AZURE KEYVAULTS
builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});
//ESTE OBJETO SOLO LO NECESITAMOS AQUI,LO DICHO,RECUPERAMOS LOS VALORES Y LOS ASIGNAMOS A UNA CLASE
//RECUPERAMOS SECRETCLIENT PARA LOS SECRETOS DE KEYVAULT
SecretClient secretClient=builder.Services.BuildServiceProvider().GetService<SecretClient>();
//ACCEDEMOS AL SECRETO
KeyVaultSecret secreto = await secretClient.GetSecretAsync("secretsqlazurekbs");


//=======SEGURIDAD JWT IMPLEMETADA=========
//CREAMOS UNA INSTANCIA DE NUESTRO HELPER
HelperActionOAuthService helper = new HelperActionOAuthService(builder.Configuration);
//ESTA INSTANCIA SOLAMENTE DEBEMOS CREARLA UNA VEZ(SINGELTON)
builder.Services.AddSingleton<HelperActionOAuthService>(helper);
builder.Services.AddSingleton<HelperCifrado>();
//HABILITAMOS LA SEGURIDAD DENTRO DE PROGRAM
builder.Services.AddAuthentication(helper.GetAuthenticationSchema()).AddJwtBearer(helper.GetJWtBearerOptions());

// Add services to the container.
//string connectionString = builder.Configuration.GetConnectionString("SqlHospital");
string connectionString = secreto.Value;
builder.Services.AddTransient<RepositoryHospital>();
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectionString));
//==============================================

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();

//DESPUÉS DE AÑADIR EL HELPER SE IMPLEMENTA ESTA LINEA ,IMPORTANTE EL ORDEN!!
app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();

app.Run();
