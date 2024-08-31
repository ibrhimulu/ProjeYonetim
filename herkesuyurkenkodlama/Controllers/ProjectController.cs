using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace herkesuyurkenkodlama.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult UserIndex()
        {
            // Giriş yapmış kullanıcının UserId'sini alıyoruz
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kullanıcının şeflik bilgilerini alıyoruz
            var userSubDepartmentId = _context.Users
                .Where(u => u.UserId == int.Parse(userId))
                .Select(u => u.SubDepartmentId)
                .FirstOrDefault();

            // Kullanıcının bağlı olduğu şefliğe ait projeleri alıyoruz
            List<ProjectViewModel> projects = _context.Projects
                .Where(p => p.SubDepartmentId == userSubDepartmentId)
                .ToList()
                .Select(x => _mapper.Map<ProjectViewModel>(x))
                .ToList();

            return View(projects);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AdminIndex()
        {
            List<ProjectViewModel> projects =
                _context.Projects.ToList()
                    .Select(x => _mapper.Map<ProjectViewModel>(x))
                    .ToList();

            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                   Value = u.UserId.ToString(),
                   Text = u.Username
                })
                .ToList();
           
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

            return View(projects);
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
        public IActionResult Create(CreateProjectModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adının benzersizliğini kontrol et
                if (_context.Projects.Any(x => x.ProjectName.ToLower() == model.ProjectName.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.ProjectName), "Bu isimli proje zaten oluşturulmuş.");

                    // Hata durumunda departman ve alt departman listelerini yeniden yükleyin
                    PopulateDepartmentsAndSubDepartments();
                    return View(model);
                }

                // Kullanıcı modelini User nesnesine dönüştür ve şifreyi hashle
                Project project = _mapper.Map<Project>(model);
                
                project.CreatedAt = DateTime.Now;
               
                // Kullanıcıyı veritabanına ekle
                _context.Projects.Add(project);
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
            Project project = _context.Projects.Find(id);

            EditProjectModel model = _mapper.Map<EditProjectModel>(project);

            PopulateDepartmentsAndSubDepartments();

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, EditProjectModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Projects.Any(x => x.ProjectName.ToLower() == model.ProjectName.ToLower() && x.ProjectId != id))
                {
                    ModelState.AddModelError(nameof(model.ProjectName), "Bu kullanıcı adı zaten kullanılıyor.");
                    PopulateDepartmentsAndSubDepartments(); // Hata durumunda departmanları yeniden yükle
                    return View(model);
                }

                // Veritabanından mevcut kullanıcıyı bul
                Project project = _context.Projects.Find(id);

                // CreatedAt ve ProfileImagePath değerlerini sakla
                var existingCreatedAt = project.CreatedAt;                

                // Model verilerini kullanıcıya map'le
                _mapper.Map(model, project);

                // Eski CreatedAt ve ProfileImagePath değerlerini geri yükle
                project.CreatedAt = existingCreatedAt;               

                // Değişiklikleri kaydet
                _context.SaveChanges();

                return RedirectToAction(nameof(AdminIndex));
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

        //Müdürlüğe bağlı şeflikleri getirmek için kullanılan Ajax kodları 
        public JsonResult GetSubDepartments(int departmentId)
        {
            var subDepartments = _context.Sdepartments
                .Where(s => s.DepartmentId == departmentId)
                .Select(s => new { s.SubDepartmentId, s.SubDepartmentName })
                .ToList();

            return Json(subDepartments);
        }

        //Şefliğe bağlı kullanıcıları getirmek için kullanılan Ajax kodları 
        public JsonResult GetUsersBySubDepartment(int subDepartmentId)
        {
            var users = _context.Users
                .Where(u => u.SubDepartmentId == subDepartmentId)
                .Select(u => new { u.UserId, u.Username })
                .ToList();

            return Json(users);
        }

    }
}
