using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollegeWebApplication.Controllers
{
    public class CityMasterADOIQController : Controller
    {
        private readonly ICityMasterADORepository _cityMasterADORepository;
        public CityMasterADOIQController(ICityMasterADORepository cityMasterADORepository)
        {
            _cityMasterADORepository = cityMasterADORepository;
        }
        public IActionResult Index()
        {
            var cityList = _cityMasterADORepository.GetAllCitiesAsync();
            return View("CityList", cityList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.States = _cityMasterADORepository.GetStatesAsync();
            return View("CreateCity");
        }

        [HttpPost]
        public IActionResult Create(CityMaster cityMaster)
        {
            if (ModelState.IsValid)
            {
                _cityMasterADORepository.AddCityAsync(cityMaster);
                return RedirectToAction("Index");
            }
            ViewBag.States = _cityMasterADORepository.GetStatesAsync();
            return View(cityMaster);
        }
    } 
}
