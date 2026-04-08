using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System;

var builder = WebApplication.CreateBuilder(args);
//=======SEGURIDAD JWT IMPLEMETADA=========
//CREAMOS UNA INSTANCIA DE NUESTRO HELPER
HelperActionOAuthService helper = new HelperActionOAuthService(builder.Configuration);
//ESTA INSTANCIA SOLAMENTE DEBEMOS CREARLA UNA VEZ(SINGELTON)
builder.Services.AddSingleton<HelperActionOAuthService>(helper);
builder.Services.AddSingleton<HelperCifrado>();
//HABILITAMOS LA SEGURIDAD DENTRO DE PROGRAM
builder.Services.AddAuthentication(helper.GetAuthenticationSchema()).AddJwtBearer(helper.GetJWtBearerOptions());

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("SqlHospital");
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
