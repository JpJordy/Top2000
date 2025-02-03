using Microsoft.AspNetCore.Mvc;

namespace Top2000_MVC.Controllers
{
    public class PodcastsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
