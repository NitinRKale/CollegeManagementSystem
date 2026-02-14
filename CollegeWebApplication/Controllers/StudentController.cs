using CollegeWebApplication.Data;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollegeWebApplication.Controllers
{
    public class StudentController : Controller
    {
        private readonly CollegeWebDbContext _context;
        public const string SessionKeyName = "UserName";
        public StudentController(CollegeWebDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var students = _context.StudentsInfo.ToList();
            return View("StudentList",students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(StudentInfo student)
        {
            if (ModelState.IsValid)
            {
                _context.StudentsInfo.Add(student);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }
    }
}
