using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Mvc;

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
            List<TeamViewModel> teams =
               _context.Sdepartments.ToList()
                   .Select(x => _mapper.Map<TeamViewModel>(x)).ToList();

            return View(teams);
        }

        public IActionResult AdminIndex()
        {
            List<TeamViewModel> teams =
                _context.Sdepartments.ToList()
                    .Select(x => _mapper.Map<TeamViewModel>(x)).ToList();

            return View(teams);           
        }

    }
}
