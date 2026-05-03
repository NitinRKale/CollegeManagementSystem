using CollegeWebApplication.Data;
using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace CollegeWebApplication.Controllers
{
    public class StateMasterEFAjaxController : Controller
    {
        private readonly CollegeWebDbContext _context;
        private readonly ILogger<StateMasterEFAjaxController> _logger;
        public StateMasterEFAjaxController(CollegeWebDbContext context,
            ILogger<StateMasterEFAjaxController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateState");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StateMaster stateMaster)
        {
            if (stateMaster == null)
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

                 await _context.StateMaster.AddAsync(stateMaster);
                 await _context.SaveChangesAsync();
                _logger.LogInformation("Student details Added {@Model} -", stateMaster);

                return Json(new
                {
                    success = true,
                    result = "Saved",
                    message = "State created successfully.",
                    ErrorMessage = "",
                    redirectTo = Url.Action("Index", "StateMasterEFAjax")
                });
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                _logger.LogError(ex, "Error on adding StateMaster {@Model}", stateMaster);
                return Json(new { success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var state = _context.StateMaster.Find(id);
            if (state == null)
            {
                _logger.LogWarning("No state found for StateId: {StateId}", id);
                return RedirectToAction(nameof(Index));
            }           

            return View("EditState", state);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StateMaster stateMaster)
        {
            if (stateMaster == null)
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
                            "Model validation failed for StateMaster. Errors: {@Errors}, Input: {@StateMaster}",
                             errors,
                             stateMaster
                        );
                    return Json(new { success = false, message = "Validation failed.", errors });
                }

                  _context.StateMaster.Update(stateMaster);
                 await _context.SaveChangesAsync();

                _logger.LogInformation(
                            "StateMaster updated successfully. StateId: {StateId}, Name: {StateName}",
                            stateMaster.StateId,
                            stateMaster.StateName
                        );

                return Json(new
                {
                    success = true,
                    result = "Saved",
                    message = "State updated successfully.",
                    ErrorMessage = "",
                    redirectTo = Url.Action("Index", "StateMasterEFAjax")
                });
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                _logger.LogError(ex, "Error updating StateMaster {@Model}", stateMaster);
                return Json(new { success = false, ErrorMessage = ex.Message });
            }
        }



        // Helper method to get the list of states for AJAX calls
        [HttpGet]
        public IActionResult GetStateList()
        {
            var stateMasters = _context.StateMaster.ToList();

            return Json(new { data = stateMasters });
        }
    }
}
