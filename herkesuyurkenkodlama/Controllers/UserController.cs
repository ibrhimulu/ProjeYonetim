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

            // Pass the lists to the view
            ViewBag.Departments = departments;
            ViewBag.SubDepartments = subDepartments;

            return View();
        }



        [HttpPost]
        public IActionResult Create(CreateUserModel model)
        {

            if (ModelState.IsValid)
            {

                if (_context.Users.Any(x => x.Username.ToLower() == model.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.Username), "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(model);
                }

                User user = _mapper.Map<User>(model);
                string hashedPassword = DoMD5HashedString(model.Password);
                user.Password = hashedPassword;
                user.CreatedAt = DateTime.Now;
                user.ProfileImagePath = "uploads/no-image.jpg";

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
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
