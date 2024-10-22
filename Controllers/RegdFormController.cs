using Application.DAL.Dashboard;
using Application.DAL.Librerian;
using Application.DAL.Student;
using Application.DAL.StudentNew;
using Application.Models.Librerian;
using Application.Models.Student;
using Application.Models.StudentNew;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CoreRegistrationApp.Controllers
{
    public class RegdFormController : Controller
    {
        private IStudentDAL _std;
        private IStudentNewDAL _stdnew;
        private IDashboardDAL _dashboard;
        private ILibrerianDAL _librian;
        public RegdFormController(IStudentDAL std, IStudentNewDAL stdnew, IDashboardDAL dashboard, ILibrerianDAL librian)
        {
            _std = std;
            _stdnew = stdnew;
            _dashboard = dashboard;
            _librian = librian;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ViewAllData()
        {
            try
            {
                var data = await _std.Get();
                return View(data);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile filenamee)
        {
            try
            {
                if (filenamee == null || filenamee.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var filePath = Path.Combine("Uploads", filenamee.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await filenamee.CopyToAsync(stream);
                }
                TempData["imgname"] = filenamee.FileName;
                TempData.Keep();
                return Json(new {success=true,message = "File uploaded successfully.", fileName = filenamee.FileName });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertData(AddUserDetails details)
        {
            try
            {
                details.image_name = TempData["imgname"] != null ? TempData["imgname"].ToString() : "";
                var data = await _std.InsertData(details);
                if(data==1)
                {
                    return Json(new { success = true, message = "Data Registered Successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Error" });
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> Download(string filename)
        {           
            if (filename == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine("Uploads", filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", filename);
        }

        public async Task<IActionResult> Dashboard()
        {
            var datadashboard = await _dashboard.GetDashboard();
            if(datadashboard != null)
            {
                ViewBag.Totaluser = datadashboard.FirstOrDefault()?.Totaluser;
                ViewBag.Activeuser = datadashboard.FirstOrDefault()?.Activeuser;
                ViewBag.DeActiveuser = datadashboard.FirstOrDefault()?.DeActiveuser;
                ViewBag.NoofStudent = datadashboard.FirstOrDefault()?.NoofStudent;
                ViewBag.NoofLibrerian = datadashboard.FirstOrDefault()?.NoofLibrerian;
                ViewBag.NoofMaleStudent = datadashboard.FirstOrDefault()?.NoofMaleStudent;
                ViewBag.NoofFemaleStudent = datadashboard.FirstOrDefault()?.NoofFemaleStudent;
                ViewBag.NoofMaleLibrerian = datadashboard.FirstOrDefault()?.NoofMaleLibrerian;
                ViewBag.NoofFemaleLibrerian = datadashboard.FirstOrDefault()?.NoofFemaleLibrerian;
            }
            return View();
        }
        public IActionResult StudentRegd()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> StudentRegdAdd(AddStudentNewModel details)
        {
            try
            {
                var data = await _stdnew.InsertStudentNewData(details);
                if (data == 1)
                {
                    return Json(new { success = true, message = "Student Added Successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Error" });
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IActionResult> StudentRegdView()
        {
            try
            {
                var data = await _stdnew.GetStudentDetailsNew();
                return View(data);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IActionResult LibrerianRegd()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LibrerianRegdAdd(AddLibrerian details)
        {
            try
            {
                var data = await _librian.InsertLibrerianData(details);
                if (data == 1)
                {
                    return Json(new { success = true, message = "Libreian Added Successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Error" });
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IActionResult> LibrerianRegdView()
        {
            try
            {
                var data = await _librian.GetLibrerianDetails();
                return View(data);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CheckLogIn(UserdetailsModel details)
        {
            if(details!=null)
            {
                var getUserdetails = await _std.GetUserDetails();
                var userdetailsfiltered = getUserdetails.Where(s => s.username == details.username && s.password == details.password).FirstOrDefault();
                bool existUser = getUserdetails.Any(s => s.username == details.username && s.password == details.password);
                if (existUser) 
                {
                    HttpContext.Session.SetString("username", userdetailsfiltered.username);
                    HttpContext.Session.SetString("name", userdetailsfiltered.fullname);
                    return Json(new { success = true});
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View();
                    //return Json(new { success = false, message = "User Not Exist" });
                }

            }
            return View();
        }
    }
}
