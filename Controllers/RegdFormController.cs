using Application.DAL.Student;
using Application.Models.Student;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<IActionResult> InsertData(AddStudentDetails details)
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
    }
}
