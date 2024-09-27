using Microsoft.AspNetCore.Mvc;

namespace CoreRegistrationApp.Controllers
{
    public class RegdFormController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
