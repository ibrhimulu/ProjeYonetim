using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using Microsoft.AspNetCore.Mvc;
using herkesuyurkenkodlama.Models;
using NETCore.Encrypt.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjeYonetim.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<UserViewModel> users =
                _context.Users.ToList()
                    .Select(x => _mapper.Map<UserViewModel>(x)).ToList();

            return View(users);
        }

        public IActionResult Create()
        {
            // Departman ve Alt Departmanları çekip ViewBag'e yükle
            var departments = _context.Mdepartments
                                       .Select(d => new SelectListItem
                                       {
                                           Value = d.DepartmentId.ToString(),
                                           Text = d.DepartmanName
                                       })
                                       .ToList();

            var subDepartments = _context.Sdepartments
                                          .Select(sd => new SelectListItem
                                          {
                                              Value = sd.SubDepartmentId.ToString(),
                                              Text = sd.SubDepartmentName
                                          })
                                          .ToList();

            ViewBag.Departments = departments;
            ViewBag.SubDepartments = subDepartments;

            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adının benzersizliğini kontrol et
                if (_context.Users.Any(x => x.Username.ToLower() == model.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.Username), "Bu kullanıcı adı zaten kullanılıyor.");

                    // Hata durumunda departman ve alt departman listelerini yeniden yükleyin
                    PopulateDepartmentsAndSubDepartments();
                    return View(model);
                }

                // Kullanıcı modelini User nesnesine dönüştür ve şifreyi hashle
                User user = _mapper.Map<User>(model);
                string hashedPassword = DoMD5HashedString(model.Password);
                user.Password = hashedPassword;
                user.CreatedAt = DateTime.Now;
                user.ProfileImagePath = "uploads/no-image.jpg";

                // Kullanıcıyı veritabanına ekle
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // Validasyon hatası durumunda departman ve alt departman listelerini yeniden yükleyin
            PopulateDepartmentsAndSubDepartments();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            User user = _context.Users.Find(id);

            EditUserModel model = _mapper.Map<EditUserModel>(user);

            PopulateDepartmentsAndSubDepartments();

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, EditUserModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(x => x.Username.ToLower() == model.Username.ToLower() && x.UserId != id))
                {
                    ModelState.AddModelError(nameof(model.Username), "Bu kullanıcı adı zaten kullanılıyor.");
                    PopulateDepartmentsAndSubDepartments(); // Hata durumunda departmanları yeniden yükle
                    return View(model);
                }

                // Veritabanından mevcut kullanıcıyı bul
                User user = _context.Users.Find(id);

                // CreatedAt ve ProfileImagePath değerlerini sakla
                var existingCreatedAt = user.CreatedAt;
                var existingProfileImagePath = user.ProfileImagePath;

                // Model verilerini kullanıcıya map'le
                _mapper.Map(model, user);

                // Eski CreatedAt ve ProfileImagePath değerlerini geri yükle
                user.CreatedAt = existingCreatedAt;
                user.ProfileImagePath = existingProfileImagePath;

                // Değişiklikleri kaydet
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            PopulateDepartmentsAndSubDepartments(); // Validasyon hatası durumunda departmanları yeniden yükle
            return View(model);
        }



        // Departman ve Alt Departman listelerini doldurmak için bir yardımcı metot
        private void PopulateDepartmentsAndSubDepartments()
        {
            ViewBag.Departments = _context.Mdepartments
                                          .Select(d => new SelectListItem
                                          {
                                              Value = d.DepartmentId.ToString(),
                                              Text = d.DepartmanName
                                          })
                                          .ToList();

            ViewBag.SubDepartments = _context.Sdepartments
                                             .Select(sd => new SelectListItem
                                             {
                                                 Value = sd.SubDepartmentId.ToString(),
                                                 Text = sd.SubDepartmentName
                                             })
                                             .ToList();
        }

        private string DoMD5HashedString(string s)
        {
            string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
            string salted = s + md5Salt;    /*şifreye indirdiğim metodu (md5) ekledim   */
            string hashed = salted.MD5();        /*database hashed olarak yazdırdım (şifrelenmiş hali)*/
            return hashed;
        }
    }
}
