using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            List<ProjectViewModel> projects =
                _context.Projects.ToList()
                    .Select(x => _mapper.Map<ProjectViewModel>(x)).ToList();

            return View();
        }
        public IActionResult AdminIndex()
        {
            List<ProjectViewModel> projects =
                _context.Projects.ToList()
                    .Select(x => _mapper.Map<ProjectViewModel>(x)).ToList();            
            
            return View(projects);
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
