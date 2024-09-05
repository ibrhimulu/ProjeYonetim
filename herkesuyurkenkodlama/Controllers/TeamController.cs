using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace herkesuyurkenkodlama.Controllers
{
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TeamController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult UserIndex()
        {
            // Giriş yapmış kullanıcının ID'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
            {
                // ID dönüşüm hatası
                return BadRequest("Geçersiz kullanıcı ID'si.");
            }

            // Giriş yapmış kullanıcının bağlı olduğu şefliğin ID'sini al
            var userSubDepartmentId = _context.Users
                .Where(u => u.UserId == userIdInt)
                .Select(u => u.SubDepartmentId)
                .FirstOrDefault();

            // Şefliğe ait tüm kullanıcıları listele
            var usersInSubDepartment = _context.Users
                .Where(u => u.SubDepartmentId == userSubDepartmentId && u.IsActive == true)
                .Select(u => new UserViewModel
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    ProfileImagePath = Path.GetFileName(u.ProfileImagePath), // Dosya adını sadece almak için
                    RoleId = u.RoleId // Role ID'sini çekiyoruz
                })
                .ToList();

            // Şeflik adını al
            var subDepartmentName = _context.Sdepartments
                .Where(sd => sd.SubDepartmentId == userSubDepartmentId)
                .Select(sd => sd.SubDepartmentName)
                .FirstOrDefault();

            // View'a Şeflik adı ve kullanıcılar listesini gönder
            var model = Tuple.Create(subDepartmentName, usersInSubDepartment);
            return View(model);

        }


        [Authorize(Roles = "Admin")]
        public IActionResult AdminIndex()
        {
            List<TeamViewModel> teams =
                _context.Sdepartments.ToList()
                    .Select(x => _mapper.Map<TeamViewModel>(x)).ToList();

            // Departmanlar için ViewBag oluşturun
            ViewBag.Departments = _context.Mdepartments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmanName
                })
                .ToList();

            return View(teams);
        }


        [Authorize(Roles = "Admin")]
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
        public IActionResult Create(CreateTeamModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adının benzersizliğini kontrol et
                if (_context.Sdepartments.Any(x => x.SubDepartmentName.ToLower() == model.SubDepartmentName.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.SubDepartmentName), "Bu şeflik adı zaten oluşturulmuş.");

                    // Hata durumunda departman ve alt departman listelerini yeniden yükleyin
                    PopulateDepartmentsAndSubDepartments();
                    return View(model);
                }

                // Kullanıcı modelini User nesnesine dönüştür ve şifreyi hashle
                Sdepartment team = _mapper.Map<Sdepartment>(model);


                // Kullanıcıyı veritabanına ekle
                _context.Sdepartments.Add(team);
                _context.SaveChanges();

                return RedirectToAction(nameof(AdminIndex));
            }

            // Validasyon hatası durumunda departman ve alt departman listelerini yeniden yükleyin
            PopulateDepartmentsAndSubDepartments();
            return View(model);

        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            // İlgili şefliği veritabanından bul
            Sdepartment team = _context.Sdepartments.Find(id);

            // Şefliği düzenleme modeline map'le
            EditTeamModel model = _mapper.Map<EditTeamModel>(team);

            // Departman ve alt departmanları doldur
            PopulateDepartmentsAndSubDepartments();

            // Modeli view'a gönder
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, EditTeamModel model)
        {
            if (ModelState.IsValid)
            {
                // Aynı isimde başka bir şeflik olup olmadığını kontrol et
                if (_context.Sdepartments.Any(x => x.SubDepartmentName.ToLower() == model.SubDepartmentName.ToLower() && x.SubDepartmentId != id))
                {
                    ModelState.AddModelError(nameof(model.SubDepartmentName), "Bu şeflik adı zaten kullanılıyor.");
                    PopulateDepartmentsAndSubDepartments(); // Hata durumunda departmanları yeniden yükle
                    return View(model);
                }

                // Veritabanından mevcut şefliği bul
                Sdepartment team = _context.Sdepartments.Find(id);

                // Şeflik ID'sinin değiştirilmesini önlemek için mevcut ID'yi sakla
                var existingSubDepartmentId = team.SubDepartmentId;

                // Model verilerini şefliğe map'le
                _mapper.Map(model, team);

                // Eski SubDepartmentId'yi geri yükle
                team.SubDepartmentId = existingSubDepartmentId;

                // Değişiklikleri kaydet
                _context.SaveChanges();

                // Admin index sayfasına yönlendir
                return RedirectToAction(nameof(AdminIndex));
            }

            PopulateDepartmentsAndSubDepartments(); // Validasyon hatası durumunda departmanları yeniden yükle
            return View(model);
        }


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
    }
}
