using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Mvc;

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


    }
}
