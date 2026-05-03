using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollegeWebApplication.Controllers
{
    public class CityMasterADOAjaxController(IStateMasterADORepository stateMasterADORepository,
         ICityMasterADOSPRepository cityMasterADOSPRepository, ILogger<CityMasterADOAjaxController> logger) : Controller
    {
        private readonly IStateMasterADORepository _stateRepo = stateMasterADORepository;
        private readonly ICityMasterADOSPRepository _cityRepo = cityMasterADOSPRepository;
        private readonly ILogger<CityMasterADOAjaxController>? _logger = logger;
        public IActionResult Index()
        {
            return View("CityList");
        }

        [HttpGet]
        public IActionResult GetAllCities()
        {
            IEnumerable<CityMaster> cityList = (IEnumerable<CityMaster>)_cityRepo.GetAllCities();

            return Json(new { data = cityList });
        }

        [HttpGet]
        public IActionResult GetCitiesByState(int stateId)
        {
            IEnumerable<CityMaster> cityList = (IEnumerable<CityMaster>)_cityRepo.GetAllCities();

            return Json(new { data = cityList });
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.States = _stateRepo.GetAllStateMasters();            

            return View("CreateStudent");
        }


    }
}
