using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using Microsoft.AspNetCore.Mvc;
using herkesuyurkenkodlama.Models;

namespace ProjeYonetim.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            List<UserViewModel> users =
                _context.Users.ToList()
                    .Select(x => _mapper.Map<UserViewModel>(x)).ToList();

            return View(users);
        }
    }
}
