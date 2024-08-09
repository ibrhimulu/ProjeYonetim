using herkesuyurkenkodlama.Contexts;
using Microsoft.EntityFrameworkCore;

namespace herkesuyurkenkodlama
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Add DbContext to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts.UseSqlServer(builder.Configuration.GetConnectionString("PersonDatabase"));
                //opts.UseLazyLoadingProxies();  //E�er Lazy Loading kullan�yorsan�z bu sat�r� uncomment yap�n.
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}