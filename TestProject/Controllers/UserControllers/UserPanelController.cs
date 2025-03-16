using Microsoft.AspNetCore.Mvc;

namespace TestProject.Controllers.UserControllers
{
    public class UserPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
