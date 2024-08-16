using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace herkesuyurkenkodlama.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
       
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = DoMD5HashedString(model.Password);

                User user = _context.Users.SingleOrDefault(x => x.Username.ToLower() == model.Username.ToLower()
                && x.Password == hashedPassword);

                if (user != null)
                {
                    if (user.IsActive == false)
                    {
                        ModelState.AddModelError(nameof(model.Username), "Kullanıcı kilitli.");
                        return View(model);
                    }

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.NameSurname ?? string.Empty));
                    // Role tablosundan rol adını alıyoruz
                    var role = _context.Roles.FirstOrDefault(r => r.RoleId == user.RoleId);
                    if (role != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Rolename));
                    }

                    claims.Add(new Claim("Username", user.Username));

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); /*yani "Cookie"*/

                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
                }

            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(x => x.Username.ToLower() == model.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.Username), "Bu kullanıcı zaten kayıtlı.");
                    return View(model);
                }

                string hashedPassword = DoMD5HashedString(model.Password);

                User user = new()
                {
                    Username = model.Username,
                    Password = hashedPassword,
                    RoleId = 1,
                    CreatedAt = DateTime.Now,
                    ProfileImagePath = "~/uploads/no-image.jpg"
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

        private string DoMD5HashedString(string s)
        {
            string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
            string salted = s + md5Salt;
            string hashed = salted.MD5();
            return hashed;
        }

        public ActionResult Profile() 
        {
            ProfileInfoLoader();

            return View();
        }

        private void ProfileInfoLoader()
        {
            int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _context.Users.SingleOrDefault(x => x.UserId == userid);

            ViewData["NameSurname"] = user.NameSurname;
            ViewData["ProfileImage"] = user.ProfileImagePath;
        }

        [HttpPost]
        public IActionResult ProfileChangeNameSurname([Required][StringLength(50)] string? namesurname)
        {
            if (ModelState.IsValid)
            {
                int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _context.Users.SingleOrDefault(x => x.UserId == userid);

                
                user.NameSurname = namesurname;
                _context.SaveChanges();

                return RedirectToAction(nameof(Profile));
            }

            ProfileInfoLoader();
            return View("Profile");
        }

        [HttpPost]
        public IActionResult ProfileChangePassword([Required]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakterden oluşmalıdır.")]
        [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakterden oluşmalıdır.")]
        [RegularExpression(@"^(?=.*[a-zğüşöçı])(?=.*[A-ZĞÜŞÖÇİ])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-zğüşöçıĞÜŞÖÇİ\d@$!%*?&.]{6,15}$",
        ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.")] string? password)
        {
            if (ModelState.IsValid)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _context.Users.SingleOrDefault(x => x.UserId == userId);

                string hashedPassword = DoMD5HashedString(password);

                user.Password = hashedPassword;
                _context.SaveChanges();

                ViewData["result"] = "PasswordChanged";
            }

            ProfileInfoLoader();
            return View("Profile");
        }


        [HttpPost]
        public IActionResult ProfileChangeImage([Required] IFormFile file)
        {
            if (ModelState.IsValid)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _context.Users.SingleOrDefault(x => x.UserId == userId);

                // Dosya adını belirliyoruz
                string filename = $"p_{userId}.jpg";
                // Dosya yolunu belirliyoruz
                string filePath = Path.Combine("uploads", filename);

                // Tam dosya yolunu kullanarak dosyayı kaydediyoruz
                using (Stream stream = new FileStream(Path.Combine("wwwroot", filePath), FileMode.OpenOrCreate))
                {
                    file.CopyTo(stream);
                }

                // Dosya yolunu veritabanında saklıyoruz
                user.ProfileImagePath = filePath;
                _context.SaveChanges();

                // Profile sayfasına yönlendirme yapıyoruz
                return RedirectToAction(nameof(Profile));
            }

            // Eğer ModelState geçerli değilse, profil bilgilerini yeniden yükleyip sayfayı tekrar gösteriyoruz
            ProfileInfoLoader();
            return View("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
