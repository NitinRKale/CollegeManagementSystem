using CollegeWebApplication.Data;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

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
            var students = _context.StudentMaster
                .Include(st => st.StateMaster)
                .Include(st => st.CityMaster)
                .Include(st => st.CourseMaster)
                .ToList();
            // Include navigation properties if they exist on the model

            //var students = _context.Set<StudentMaster>()
            //    .AsNoTracking()
            //    .Include(sm => sm.StateMaster)
            //    .Include(sm => sm.CityMaster)
            //    .Include(sm => sm.CourseMaster)
            //    .ToList();

            _logger.LogInformation("Index returning {Count} students", students.Count);
            return View("StudentList", students);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.States = await _context.StateMaster.ToListAsync();
            ViewBag.Courses = await _context.CourseMaster.ToListAsync();

            _logger.LogInformation("Create GET requested");
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(StudentMaster student)
        {
            _logger.LogInformation("Create POST requested for student {@Student}", student);
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.StudentMaster.AddAsync(student);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Student created with id {Id}", student.StudentId);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating student {@Student}", student);
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the student.");
                }
            }
            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Edit GET requested for id {Id}", id);
            
            var student = await _context.StudentMaster.FindAsync(id);

            if (student == null)
            {
                _logger.LogWarning("Details - student not found for id {Id}", id);
                return RedirectToAction("Index");
            }

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                await _context.StateMaster.ToListAsync(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                await _context.CityMaster.Where(x => x.StateId == student.StateId).ToListAsync(), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                await _context.CourseMaster.ToListAsync(), "CourseId", "CourseName", student.CourseId);

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StudentMaster student)
        {
            _logger.LogInformation("Edit POST requested for student {@Student}", student);
            if (ModelState.IsValid)
            {
                try
                {                    
                    _context.StudentMaster.Update(student);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Student updated with id {Id}", student.StudentId);
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
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.StudentMaster.FindAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Details - student not found for id {Id}", id);
                return RedirectToAction("Index");
            }

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                await _context.StateMaster.ToListAsync(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                await _context.CityMaster.Where(x => x.StateId == student.StateId).ToListAsync(), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                await _context.CourseMaster.ToListAsync(), "CourseId", "CourseName", student.CourseId);

            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete GET requested for id {Id}", id);
            var student = await _context.StudentMaster.FindAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Delete GET - student not found for id {Id}", id);
                return RedirectToAction("Index");
            }

            // Use SelectList so tag helpers will select the current value automatically
            ViewBag.StateList = new SelectList(
                await _context.StateMaster.ToListAsync(), "StateId", "StateName", student.StateId);

            ViewBag.CityList = new SelectList(
                await _context.CityMaster.Where(x => x.StateId == student.StateId).ToListAsync(), "CityId", "CityName", student.CityId);

            ViewBag.CourseList = new SelectList(
                await _context.CourseMaster.ToListAsync(), "CourseId", "CourseName", student.CourseId);


            return View(student);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Delete POST requested for id {Id}", id);
            try
            {
                var student = await _context.StudentMaster.FindAsync(id);
                if (student != null)
                {
                    _context.StudentMaster.Remove(student);
                    await _context.SaveChangesAsync();
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

        [HttpGet]
        public async Task<JsonResult> GetCities(int stateId)
        {
            try
            {
                var cities = await _context.CityMaster.Where(x=> x.StateId == stateId).ToListAsync() ?? new List<CityMaster>();
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
