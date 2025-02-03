using Microsoft.AspNetCore.Mvc;

namespace Top2000_MVC.Controllers
{
    public class Top2000Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
