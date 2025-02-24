using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Top2000_MVC.Controllers
{
    [Authorize] 
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
