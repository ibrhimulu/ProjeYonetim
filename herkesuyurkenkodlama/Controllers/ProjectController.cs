using Microsoft.AspNetCore.Mvc;

namespace herkesuyurkenkodlama.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult UserIndex()
        {
            return View();
        }
        public IActionResult AdminIndex()
        {
            return View();
        }
    }
}
