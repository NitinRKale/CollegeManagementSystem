using CollegeWebApplication.Data;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollegeWebApplication.Controllers
{
    public class StudentController : Controller
    {
        private readonly CollegeWebDbContext _context;
        private readonly ILogger<StudentController> _logger;

        public StudentController(CollegeWebDbContext context, ILogger<StudentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Index requested - listing students");
            var students = _context.StudentsInfo.ToList();
            _logger.LogInformation("Index returning {Count} students", students.Count);
            return View("StudentList", students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Create GET requested");
            return View();
        }


        [HttpPost]
        public IActionResult Create(StudentInfo student)
        {
            _logger.LogInformation("Create POST requested for student {@Student}", student);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.StudentsInfo.Add(student);
                    _context.SaveChanges();
                    _logger.LogInformation("Student created with id {Id}", 0);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating student {@Student}", 0);
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the student.");
                }
            }
            return View(student);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Edit GET requested for id {Id}", id);
            var student = _context.StudentsInfo.Find(id);
            if (student == null)
            {
                _logger.LogWarning("Edit GET - student not found for id {Id}", id);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        [HttpPost]
        public IActionResult Edit(StudentInfo student)
        {
            _logger.LogInformation("Edit POST requested for student {@Student}", student);
            if (ModelState.IsValid)
            {
                try
                {
                    student.UpdateDate = DateTime.Now;
                    _context.StudentsInfo.Update(student);
                    _context.SaveChanges();
                    _logger.LogInformation("Student updated with id {Id}", 0);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating student {@Student}", student);
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the student.");
                }
            }
            return View(student);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var student = _context.StudentsInfo.Find(id);
            if (student == null)
            {
                _logger.LogWarning("Details - student not found for id {Id}", id);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Delete GET requested for id {Id}", id);
            var student = _context.StudentsInfo.Find(id);
            if (student == null)
            {
                _logger.LogWarning("Delete GET - student not found for id {Id}", id);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation("Delete POST requested for id {Id}", id);
            try
            {
                var student = _context.StudentsInfo.Find(id);
                if (student != null)
                {
                    _context.StudentsInfo.Remove(student);
                    _context.SaveChanges();
                    _logger.LogInformation("Student deleted with id {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Delete POST - student not found for id {Id}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting student id {Id}", id);
            }
            return RedirectToAction("Index");
        }
    }
}
