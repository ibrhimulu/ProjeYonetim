using Microsoft.AspNetCore.Mvc;

namespace herkesuyurkenkodlama.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult AdminIndex()
        {
            return View();
        }
        
        public IActionResult UserIndex()
        {
            return View();
        }
    }
}
