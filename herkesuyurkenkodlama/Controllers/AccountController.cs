using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;

namespace herkesuyurkenkodlama.Controllers
{
    public class AccountController : Controller
    {
       
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Login işlemleri
                             

            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            

                if (ModelState.IsValid)
                {
                   
                    string md5Salt= _configuration.GetValue<string>("AppSettings:MD5Salt");
                    string saltedPassword = model.Password + md5Salt;
                    string hashedPassword = saltedPassword.MD5(); 

                    User user = new()
                    {
                        Username = model.Username,
                        Password = hashedPassword,
                        RoleId = 1
                    };

                    _context.Users.Add(user);
                    int affectedRowCount = _context.SaveChanges(); /*etkilenen satır sayısı*/

                    if (affectedRowCount == 0)
                    {
                        ModelState.AddModelError("", "Bu kullanıcı eklenemez.");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Login));
                    }

                }
                return View(model);                       
        }

    }
}
