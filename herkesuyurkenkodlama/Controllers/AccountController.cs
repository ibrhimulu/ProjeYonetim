using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Mvc;

namespace herkesuyurkenkodlama.Controllers
{
    public class AccountController : Controller
    {
       
            private readonly ApplicationDbContext _context;

            public AccountController(ApplicationDbContext context)
            {
                _context = context;
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

    }
}
