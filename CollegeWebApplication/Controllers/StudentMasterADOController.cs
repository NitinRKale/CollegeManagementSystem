using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollegeWebApplication.Controllers
{
    public class StudentMasterADOController(IStudentMasterADOSPRepository studentMasterADOSPRepository,
             IStateMasterADORepository stateMasterADORepository,
             ICityMasterADOSPRepository cityMasterADOSPRepository,
             ICourseMasterADORepository courseMasterADORepository) : Controller
    {
       private readonly IStudentMasterADOSPRepository _studentRepo = studentMasterADOSPRepository;
       private readonly IStateMasterADORepository _stateRepo = stateMasterADORepository;
       private readonly ICityMasterADOSPRepository _cityRepo = cityMasterADOSPRepository;
       private readonly ICourseMasterADORepository _courseRepo = courseMasterADORepository;
       private readonly ILogger<StudentMasterEFController> _logger;

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<StudentMasterADO> studentMasters = (IEnumerable<StudentMasterADO>)_studentRepo.GetAllStudentMasters();
            if (studentMasters.Any())
            {
                return View("StudentList", studentMasters);
            }

            return View("StudentList");
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.States =  _stateRepo.GetAllStateMasters();
            ViewBag.Courses = _courseRepo.GetAllCourses();

            return View("CreateStudent");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentMasterADO model)
        {
            if (!ModelState.IsValid) return View("CreateStudent", model);

            try
            {
                string result = await _studentRepo.AddStudentMaster(model);
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
        public IActionResult Edit(int id)
        {
            var student = _studentRepo.GetStudentMasterById(id);
            if (student == null) 
            {
                return RedirectToAction(nameof(Index));
            }

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                _stateRepo.GetAllStateMasters(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                _cityRepo.GetCitiesByState(student.StateId), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                _courseRepo.GetAllCourses(), "CourseId", "CourseName", student.CourseId);

            return View("EditStudent", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentMasterADO model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                string result = await _studentRepo.UpdateStudentMaster(model);
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
        public IActionResult Details(int id)
        {
            var student = _studentRepo.GetStudentMasterById(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                _stateRepo.GetAllStateMasters(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                _cityRepo.GetCitiesByState(student.StateId), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                _courseRepo.GetAllCourses(), "CourseId", "CourseName", student.CourseId);

            return View("DetailsStudent", student);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {

            var student = _studentRepo.GetStudentMasterById(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                _stateRepo.GetAllStateMasters(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                _cityRepo.GetCitiesByState(student.StateId), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                _courseRepo.GetAllCourses(), "CourseId", "CourseName", student.CourseId);

            return View("DeleteStudent", student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var student = _studentRepo.GetStudentMasterById(id);
                // Assuming you want to delete the student here
                if (student != null)
                {
                    await _studentRepo.DeleteStudentMaster(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting StudentMaster id {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public JsonResult GetCities(int stateId)
        {
            try
            {
                var cities = _cityRepo.GetCitiesByState(stateId) ?? new List<CityMaster>();
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
