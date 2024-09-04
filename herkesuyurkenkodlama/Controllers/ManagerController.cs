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
            return View();
        }
        public IActionResult Teams()
        {
            return View();
        }
    }
}
