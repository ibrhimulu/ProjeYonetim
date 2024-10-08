﻿using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace herkesuyurkenkodlama.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TaskController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult UserIndex()
        {
            // Giriş yapmış kullanıcının ID'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kullanıcıya ait görevleri sorgula
            var tasksForUser =
                from task in _context.Tasklars
                join project in _context.Projects on task.ProjectId equals project.ProjectId
                join status in _context.Statuses on task.StatusId equals status.StatusId
                join assignedUser in _context.Users on task.AssignedUserId equals assignedUser.UserId
                join department in _context.Mdepartments on task.DepartmentId equals department.DepartmentId
                join subDepartment in _context.Sdepartments on task.SubDepartmentId equals subDepartment.SubDepartmentId
                where task.AssignedUserId == Convert.ToInt32(userId) && task.IsActive ==true
                select new TasklarViewModel
                {
                    TaskId = task.TaskId,
                    Title = task.Title,
                    ProjectId = project.ProjectId,
                    StatusId = status.StatusId,
                    DepartmentId = department.DepartmentId,
                    SubDepartmentId = subDepartment.SubDepartmentId,
                    CreatedAt = task.CreatedAt,
                    TaskComment = task.TaskComment,
                    TaskDescription = task.TaskDescription,

                    // Diğer gerekli alanlar
                };

            var userTasks = tasksForUser.ToList();


            // Eğer kullanıcının görevleri yoksa, boş bir liste gönder
            if (userTasks == null || !userTasks.Any())
            {
                userTasks = new List<TasklarViewModel>();
            }

            // Kullanıcıları ViewBag'e ekleyin
            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.Username
                })
                .ToList();

            // Müdürlükleri ViewBag'e ekleyin
            ViewBag.Departments = _context.Mdepartments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmanName
                })
                .ToList();

            // Şeflikleri ViewBag'e ekleyin
            ViewBag.SubDepartments = _context.Sdepartments
                .Select(sd => new SelectListItem
                {
                    Value = sd.SubDepartmentId.ToString(),
                    Text = sd.SubDepartmentName
                })
                .ToList();

            // Projeleri ViewBag'e ekleyin
            ViewBag.Projects = _context.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.ProjectId.ToString(),
                    Text = p.ProjectName
                })
                .ToList();

            // Durumları ViewBag'e ekleyin
            ViewBag.StatusList = _context.Statuses
                .Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.StatusName
                })
                .ToList();

            return View(userTasks);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AdminIndex()
        {
            // Taskları ViewModel'e dönüştür
            List<TasklarViewModel> tasklars =
                _context.Tasklars.ToList()
                    .Select(x => _mapper.Map<TasklarViewModel>(x)).ToList();

            // Kullanıcıları ViewBag'e ekleyin
            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.Username
                })
                .ToList();

            // Müdürlükleri ViewBag'e ekleyin
            ViewBag.Departments = _context.Mdepartments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmanName
                })
                .ToList();

            // Şeflikleri ViewBag'e ekleyin
            ViewBag.SubDepartments = _context.Sdepartments
                .Select(sd => new SelectListItem
                {
                    Value = sd.SubDepartmentId.ToString(),
                    Text = sd.SubDepartmentName
                })
                .ToList();

            // Projeleri ViewBag'e ekleyin
            ViewBag.Projects = _context.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.ProjectId.ToString(),
                    Text = p.ProjectName
                })
                .ToList();

            // Durumlar için
            ViewBag.StatusList = _context.Statuses
                  .Select(s => new SelectListItem
                  {
                      Value = s.StatusId.ToString(),
                      Text = s.StatusName
                  })
                  .ToList();

            return View(tasklars);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var projects = _context.Projects
                            .Select(p => new SelectListItem
                            {
                                Value = p.ProjectId.ToString(),
                                Text = p.ProjectName
                            })
                            .ToList();

            ViewBag.Projects = projects;

            // Departman ve Alt Departmanları çekip ViewBag'e yükle
            PopulateDepartmentsAndSubDepartments();

            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateTasklarModel model)
        {
            if (ModelState.IsValid)
            {
                // Görev adının benzersizliğini kontrol et
                if (_context.Tasklars.Any(x => x.Title.ToLower().Trim() == model.Title.ToLower().Trim()))
                {
                    ModelState.AddModelError(nameof(model.Title), "Bu görev zaten oluşturulmuş.");

                    // Hata durumunda departman ve alt departman listelerini yeniden yükleyin
                    PopulateDepartmentsAndSubDepartments();
                    return View(model);
                }

                // Task modelini Task nesnesine dönüştür
                Tasklar task = _mapper.Map<Tasklar>(model);
                task.CreatedAt = DateTime.Now; // Görevin oluşturulma zamanını ekle                

                // Görevi veritabanına ekle
                _context.Tasklars.Add(task);
                _context.SaveChanges();

                return RedirectToAction(nameof(AdminIndex)); // Başarılı bir ekleme sonrası yönlendirme
            }

            // Validasyon hatası durumunda departman ve alt departman listelerini yeniden yükleyin
            PopulateDepartmentsAndSubDepartments();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            // Veritabanından mevcut görevi bul
            Tasklar task = _context.Tasklars.Find(id);

            // Görev verilerini EditTasklarModel'e map'le
            EditTasklarModel model = _mapper.Map<EditTasklarModel>(task);

            ViewBag.StatusList = _context.Statuses
                   .Select(s => new SelectListItem
                   {
                       Value = s.StatusId.ToString(),
                       Text = s.StatusName
                   })
                   .ToList();

            // Departman ve alt departmanları doldur
            PopulateDepartmentsAndSubDepartments();
            // Projeleri doldur 
            PopulateProjectsAndStatues();

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, EditTasklarModel model)
        {
            if (ModelState.IsValid)
            {
                // Aynı isimde başka bir görev olup olmadığını kontrol et
                if (_context.Tasklars.Any(x => x.Title.ToLower() == model.Title.ToLower() && x.TaskId != id))
                {
                    ModelState.AddModelError(nameof(model.Title), "Bu görev adı zaten kullanılıyor.");

                    // Hata durumunda departmanları ve projeleri yeniden yükleyin
                    PopulateDepartmentsAndSubDepartments();
                    PopulateProjectsAndStatues();

                    return View(model);
                }

                // Veritabanından mevcut görevi bul
                Tasklar task = _context.Tasklars.Find(id);

                // Görevin oluşturulma zamanını sakla
                var existingCreatedAt = task.CreatedAt;

                // Model verilerini göreve map'le
                _mapper.Map(model, task);

                // Eski CreatedAt değerini geri yükle
                task.CreatedAt = existingCreatedAt;

                // Değişiklikleri kaydet
                _context.SaveChanges();

                return RedirectToAction(nameof(AdminIndex));
            }

            // Validasyon hatası durumunda departmanları ve projeleri yeniden yükleyin
            PopulateDepartmentsAndSubDepartments();
            PopulateProjectsAndStatues();

            return View(model);
        }


        [HttpPost]
        public IActionResult AddComment(int taskId, string newComment)
        {
            var task = _context.Tasklars.FirstOrDefault(t => t.TaskId == taskId);
            if (task != null && !string.IsNullOrEmpty(newComment))
            {
                task.TaskComment = newComment;
                _context.SaveChanges();
            }

            return RedirectToAction("UserIndex");
        }

        [HttpPost]
        public IActionResult CompleteTask(int TaskId)
        {
            var task = _context.Tasklars.FirstOrDefault(t => t.TaskId == TaskId);

            if (task == null)
            {
                return NotFound();
            }

            task.StatusId = 3; 

            _context.SaveChanges();

            return RedirectToAction("UserIndex");
        }
        [HttpPost]
        public IActionResult RevertTask(int taskId)
        {
            var task = _context.Tasklars.Find(taskId);
            if (task != null)
            {
                task.StatusId = 2;
                _context.SaveChanges();
            }

            return RedirectToAction("UserIndex");
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
        private void PopulateProjectsAndStatues()
        {
            ViewBag.Projects = _context.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.ProjectId.ToString(),
                    Text = p.ProjectName
                })
                .ToList();

            ViewBag.StatusList = _context.Statuses
                 .Select(s => new SelectListItem
                 {
                     Value = s.StatusId.ToString(),
                     Text = s.StatusName
                 })
                 .ToList();
        }

        // Müdürlüğe bağlı şeflikleri getirmek için kullanılan Ajax kodları 
        public JsonResult GetSubDepartments(int departmentId)
        {
            var subDepartments = _context.Sdepartments
                .Where(s => s.DepartmentId == departmentId)
                .Select(s => new { s.SubDepartmentId, s.SubDepartmentName })
                .ToList();

            return Json(subDepartments);
        }

        // Şefliğe bağlı kullanıcıları getirmek için kullanılan Ajax kodları 
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