using Microsoft.AspNetCore.Authentication.Cookies;
using MvcOAuthEmpleados.Services;

var builder = WebApplication.CreateBuilder(args);
//inyectamos el httpcontextaccesor
builder.Services.AddHttpContextAccessor();

//EN PROGRAM HABILITAMOS TODO SESSION E INYECTAMOS SERVICIOS

builder.Services.AddTransient<ServiceEmpleados>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

// Add services to the container.
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting=false);//redireccion manual

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseRouting(); quitamos routing y mapstatic y AÑADIMO STATIC Y AUTHENTICATION y cambiamos mapcontrollerroute
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

//app.MapStaticAssets();
//SIEMPRE USAR APP.USE SESSION SI LO USAMOOS
app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(name: "default",
        template: "{controller=Home}/{action=Index}/{id?}"
        );
});

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();


app.Run();
