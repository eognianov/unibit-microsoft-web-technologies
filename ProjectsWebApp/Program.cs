using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.Data;
using ProjectsWebApp.Services;
using ProjectsWebApp.Services.Contracts;

namespace ProjectsWebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<AppDbContext>(options =>
        {

            options.UseSqlite(builder.Configuration.GetConnectionString("SQLite"));
            options.UseLazyLoadingProxies();
        });
        builder.Services.AddScoped<IStudentsService, StudentsService>();
        builder.Services.AddScoped<IProjectsService, ProjectsService>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            // app.UseHsts();
        }

        // app.UseHttpsRedirection();
        app.UseStaticFiles();
        
        // app.Use(async (context, next) =>
        // {
        //     Console.WriteLine("Before");
        //     Console.WriteLine(context.Request.Path);
        //     context.Response.Headers.Append("Some-Header", "Test Header");
        //     await next.Invoke();
        //     Console.WriteLine("After");
        // });

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Projects}/{action=Index}/{id?}");

        app.Run();
    }
}