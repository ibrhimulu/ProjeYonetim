using herkesuyurkenkodlama.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
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

            builder.Services
               .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(opts =>
               {
                   opts.Cookie.Name = "herkesuyurkenkodlama.auth";          /*Cookie adi*/
                   opts.ExpireTimeSpan = TimeSpan.FromDays(7);    /*Cookie kalma s�resi*/
                   opts.SlidingExpiration = false;
                   opts.LoginPath = "/Account/Login";
                   opts.LogoutPath = "/Account/Logout";
                   opts.AccessDeniedPath = "/Home/AccessDenied";
               });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}