using AutoMapper;
using herkesuyurkenkodlama.Contexts;
using herkesuyurkenkodlama.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace herkesuyurkenkodlama.Controllers
{
    public class HeadshipController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HeadshipController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            List<HeadshipViewModel> teams =
                _context.Mdepartments.ToList()
                    .Select(x => _mapper.Map<HeadshipViewModel>(x)).ToList();

            return View(teams);
        }

        public IActionResult Create()
        {           
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateHeadshipModel model)
        {
            if (ModelState.IsValid)            {
                
                if (_context.Mdepartments.Any(x => x.DepartmanName.ToLower() == model.DepartmanName.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.DepartmanName), "Bu müdürlük adı zaten kullanılıyor.");

                   
                    return View(model);
                }
                
                Mdepartment mdepartment = _mapper.Map<Mdepartment>(model);

                mdepartment.IsActive = model.IsActive ?? true;

                _context.Mdepartments.Add(mdepartment);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            Mdepartment mdepartment = _context.Mdepartments.Find(id);

            if (mdepartment == null)
            {
                return NotFound();
            }

            EditHeadshipModel model = _mapper.Map<EditHeadshipModel>(mdepartment);

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, EditHeadshipModel model)
        {
            if (ModelState.IsValid)
            {
                // Aynı isimde başka bir müdürlük var mı kontrolü
                if (_context.Mdepartments.Any(x => x.DepartmanName.ToLower() == model.DepartmanName.ToLower() && x.DepartmentId != id))
                {
                    ModelState.AddModelError(nameof(model.DepartmanName), "Bu müdürlük adı zaten kullanılıyor.");
                    return View(model);
                }

                Mdepartment mdepartment = _context.Mdepartments.Find(id);

                if (mdepartment == null)
                {
                    return NotFound();
                }

                // Sadece güncellenmesi gereken alanları güncelle
                mdepartment.DepartmanName = model.DepartmanName;
                mdepartment.IsActive = model.IsActive;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


    }
}
