using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")] // Alleen Admins mogen hier komen
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
