using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace herkesuyurkenkodlama.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ManagerController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Projects()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Geçersiz kullanıcı ID.");
            }

            var userDepartmentId = _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.DepartmentId)
                .FirstOrDefault();

            var projects = _context.Projects
                .Where(p => p.DepartmentId == userDepartmentId && p.IsActive == true)
                .ToList()
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

        public IActionResult Tasks()
        {
            // Giriş yapmış kullanıcının ID'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kullanıcının müdürlüğünü al
            var userDepartmentId = _context.Users
                .Where(u => u.UserId == Convert.ToInt32(userId))
                .Select(u => u.DepartmentId)
                .FirstOrDefault();

            // Kullanıcının müdürlüğüne ait görevleri sorgula
            var tasksForUserDepartment =
                from task in _context.Tasklars
                join project in _context.Projects on task.ProjectId equals project.ProjectId
                join status in _context.Statuses on task.StatusId equals status.StatusId
                join assignedUser in _context.Users on task.AssignedUserId equals assignedUser.UserId
                join department in _context.Mdepartments on task.DepartmentId equals department.DepartmentId
                join subDepartment in _context.Sdepartments on task.SubDepartmentId equals subDepartment.SubDepartmentId
                where task.DepartmentId == userDepartmentId && task.IsActive == true
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
                    AssignedUserId = task.AssignedUserId, // Bu satırı ekleyin
                                                          // Diğer gerekli alanlar
                };

            var userTasks = tasksForUserDepartment.ToList();

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


        public IActionResult Teams()
        {
            return View();
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

            return RedirectToAction("Tasks");
        }

    }
}
