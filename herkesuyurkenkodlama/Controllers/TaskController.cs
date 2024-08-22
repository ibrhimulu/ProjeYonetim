using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Mvc;

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
            List<TasklarViewModel> tasklars =
               _context.Tasklars.ToList()
                   .Select(x => _mapper.Map<TasklarViewModel>(x)).ToList();

            return View();
        }

        public IActionResult AdminIndex()
        {
            List<TasklarViewModel> tasklars =
               _context.Tasklars.ToList()
                   .Select(x => _mapper.Map<TasklarViewModel>(x)).ToList();

            return View(tasklars);            
        }

       
    }
}
