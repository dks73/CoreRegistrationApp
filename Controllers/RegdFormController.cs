using Application.DAL.Student;
using Microsoft.AspNetCore.Mvc;

namespace CoreRegistrationApp.Controllers
{
    public class RegdFormController : Controller
    {
        private IStudent _std;
        public RegdFormController(IStudent std)
        {
            _std = std;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ViewAllData()
        {
            try
            {
                var data = _std.Get();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            return View();
        }
    }
}
