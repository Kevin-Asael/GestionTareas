using GestionTareas.API.Consumer;
using GestionTareas.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GestionTareas.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Crud<Usuario>.EndPoint = "https://localhost:7292/api/Usuarios";
            Crud<Seguimiento>.EndPoint = "https://localhost:7292/api/Seguimientos";
            Crud<Tarea>.EndPoint = "https://localhost:7292/api/Tareas";
            Crud<Proyecto>.EndPoint = "https://localhost:7292/api/Proyectos";
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/AuthV/Login";  
                    options.LogoutPath = "/AuthV/Logout"; 
                    options.AccessDeniedPath = "/AuthV/AccessDenied"; 
                });

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=AuthV}/{action=Login}/{id?}"); 

            app.Run();
        }
    }
}
