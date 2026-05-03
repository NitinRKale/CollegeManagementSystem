using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace CollegeWebApplication.Controllers
{
    public class StudentMasterADOAjaxController : Controller
    {
        private readonly IStudentMasterADOSPRepository _studentRepo;
        private readonly IStateMasterADORepository _stateRepo;
        private readonly ICityMasterADOSPRepository _cityRepo;
        private readonly ICourseMasterADORepository _courseRepo;
        private readonly ILogger<StudentMasterADOAjaxController> _logger;

        public StudentMasterADOAjaxController(
            IStudentMasterADOSPRepository studentMasterADOSPRepository,
            IStateMasterADORepository stateMasterADORepository,
            ICityMasterADOSPRepository cityMasterADOSPRepository,
            ICourseMasterADORepository courseMasterADORepository,
            ILogger<StudentMasterADOAjaxController> logger)
        {
            _studentRepo = studentMasterADOSPRepository;
            _stateRepo = stateMasterADORepository;
            _cityRepo = cityMasterADOSPRepository;
            _courseRepo = courseMasterADORepository;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View("StudentList");
        }

        [HttpGet]
        public IActionResult GetStudentList()
        {
            IEnumerable<StudentMasterADO> studentMasters = (IEnumerable<StudentMasterADO>)_studentRepo.GetAllStudentMasters();

            return Json(new { data = studentMasters });
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.States = _stateRepo.GetAllStateMasters();
            ViewBag.Courses = _courseRepo.GetAllCourses();

            return View("CreateStudent");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentMasterADO student)
        {
            if (student == null)
            {
                return Json(new { success = false, message = "Invalid request payload. Make sure JSON is sent and property names match the model." });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var entry in ModelState)
                    {
                        foreach (var err in entry.Value.Errors)
                        {
                            // prefer the explicit error message, fall back to exception message
                            errors.Add(string.IsNullOrWhiteSpace(err.ErrorMessage) ? (err.Exception?.Message ?? "") : err.ErrorMessage);
                        }
                    }
                    _logger.LogCritical("Model validation fail for {@Model} - ", errors);

                    return Json(new { success = false, message = "Validation failed.", errors });
                }

                string result = await _studentRepo.AddStudentMaster(student);
               _logger.LogInformation("Student details Added {@Model} -", student);

                return Json(new
                {
                    success = true,
                    result = "Saved",
                    message = "Student created successfully.",
                    ErrorMessage = "",
                    redirectTo = Url.Action("Index", "StudentMasterADOAjax")
                });
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                _logger.LogError(ex, "Error on adding StudentMaster {@Model}", student);
                return Json(new { success = false, ErrorMessage = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _studentRepo.GetStudentMasterById(id);
            if (student == null)
            {
                _logger.LogWarning("No student found for StudentId: {StudentId}", id);
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
        public async Task<IActionResult> Edit(StudentMasterADO student)
        {
            if (student == null)
            {
                return Json(new { success = false, message = "Invalid request payload. Make sure JSON is sent and property names match the model." });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var entry in ModelState)
                    {
                        foreach (var err in entry.Value.Errors)
                        {
                            // prefer the explicit error message, fall back to exception message
                            errors.Add(string.IsNullOrWhiteSpace(err.ErrorMessage) ? (err.Exception?.Message ?? "") : err.ErrorMessage);
                        }
                    }
                    _logger.LogWarning(
                            "Model validation failed for Student. Errors: {@Errors}, Input: {@Student}",
                             errors,
                             student
                        );
                    return Json(new { success = false, message = "Validation failed.", errors });
                }

                string result = await _studentRepo.UpdateStudentMaster(student);
                _logger.LogInformation(
                            "Student updated successfully. StudentId: {StudentId}, Name: {FullName}",
                            student.StudentId,
                            $"{student.FirstName} {student.LastName}"
                        );

                return Json(new
                {
                    success = true,
                    result = "Saved",
                    message = "Student updated successfully.",
                    ErrorMessage = "",
                    redirectTo = Url.Action("Index", "StudentMasterADOAjax")
                });
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                _logger.LogError(ex, "Error updating StudentMaster {@Model}", student);
                return Json(new { success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var student = _studentRepo.GetStudentMasterById(id);
            if (student == null)
            {
                _logger.LogWarning("[StudentMasterADOAjaxController] [Details] - No student found for StudentId: {StudentId}", id);
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
                _logger.LogWarning("[StudentMasterADOAjaxController] [Delete] - No student found for StudentId: {StudentId}", id);
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
        public async Task<IActionResult> DeleteConfirmed(int studentId)
        {
            try
            {
                if (studentId <= 0)
                {
                    _logger.LogWarning("[StudentMasterADOAjaxController] [DeleteConfirmed] - No student found for StudentId: {StudentId}", studentId);
                    return Json(new { success = false, message = "Error encountered while delete employee." });
                }

                var student = _studentRepo.GetStudentMasterById(studentId);

                if (student == null)
                {
                    _logger.LogWarning("[StudentMasterADOAjaxController] [DeleteConfirmed] - No student found for StudentId: {StudentId}", studentId);
                    return Json(new { success = false, message = "Employee details was not found" });
                }

                string result = await _studentRepo.DeleteStudentMaster(studentId);

                return Json(new
                {
                    success = true,
                    result = "Deleted",
                    message = "Student deleted successfully.",
                    ErrorMessage = "",
                    redirectTo = Url.Action("Index", "StudentMasterADOAjax")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = "Error", ErrorMessage = ex.Message });
            }
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
                _logger.LogError(ex, "Error getting cities for stateId {StateId}", stateId);
                return Json(new List<CityMaster>());
            }
        }

    }
}
