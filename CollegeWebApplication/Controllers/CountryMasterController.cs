using CollegeWebApplication.Data;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollegeWebApplication.Controllers
{
    public class CountryMasterController : Controller
    {
        private readonly CollegeWebDbContext _context;

        public CountryMasterController(CollegeWebDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var countries = _context.CountryMaster.ToList();
            return View(countries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CountryMaster countryMaster)
        {
            if (ModelState.IsValid)
            {
                _context.CountryMaster.Add(countryMaster);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(countryMaster);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var country = _context.CountryMaster.Find(id);
            if (country == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        [HttpPost]
        public IActionResult Edit(CountryMaster countryMaster)
        {
            if (ModelState.IsValid)
            {
                _context.CountryMaster.Update(countryMaster);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(countryMaster);
        }

    }
}
