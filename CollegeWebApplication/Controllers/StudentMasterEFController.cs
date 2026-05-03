using CollegeWebApplication.Filters;
using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollegeWebApplication.Controllers
{
   // [CustomAuthorize("User,Admin,SuperAdmin")]
    public class StudentMasterEFController : Controller
    {
        private readonly IStudentMasterEFRepository _repository;
        private readonly ILogger<StudentMasterEFController> _logger;

        public StudentMasterEFController(IStudentMasterEFRepository repository, ILogger<StudentMasterEFController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _repository.GetAllStudentMastersAsync();
            return View("StudentList",list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.States = await _repository.GetAllStateAsync();
            ViewBag.Courses = await _repository.GetAllCourseAsync();

            return View("CreateStudent");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentMaster model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _repository.AddStudentMasterAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating StudentMaster {@Model}", model);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the record.");
                return View("CreateStudent", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _repository.GetStudentMasterByIdAsync(id);
            if (student == null) return RedirectToAction(nameof(Index));

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                await _repository.GetAllStateAsync(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                await _repository.GetAllCityAsync(student.StateId), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                await _repository.GetAllCourseAsync(), "CourseId", "CourseName", student.CourseId);

            return View("EditStudent",student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentMaster model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _repository.UpdateStudentMasterAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating StudentMaster {@Model}", model);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the record.");
                return View("EditStudent", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var student = await _repository.GetStudentMasterByIdAsync(id);
            if (student == null) return RedirectToAction(nameof(Index));

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                await _repository.GetAllStateAsync(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                await _repository.GetAllCityAsync(student.StateId), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                await _repository.GetAllCourseAsync(), "CourseId", "CourseName", student.CourseId);

            return View("DetailStudent", student);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _repository.GetStudentMasterByIdAsync(id);
            if (student == null) return RedirectToAction(nameof(Index));

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                await _repository.GetAllStateAsync(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                await _repository.GetAllCityAsync(student.StateId), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                await _repository.GetAllCourseAsync(), "CourseId", "CourseName", student.CourseId);
            return View("DeleteStudent", student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _repository.DeleteStudentMasterAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting StudentMaster id {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetCities(int stateId)
        {
            try
            {
                var cities = await _repository.GetAllCityAsync(stateId) ?? new List<CityMaster>();
                return Json(cities);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting cities for stateId {StateId}", stateId);
                return Json(new List<CityMaster>());
            }
        }
    }
}