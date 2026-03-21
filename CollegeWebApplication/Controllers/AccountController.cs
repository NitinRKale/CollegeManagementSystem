using CollegeWebApplication.Data;
using CollegeWebApplication.Models;
using CollegeWebApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollegeWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Email or password is incorrect.");
        //            return View(model);
        //        }
        //    }
        //    return View(model);
        //}


        //[HttpGet]
        //public IActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //ApplicationUser users = new ApplicationUser
        //        //{
        //        //    FirstName = model.FirstName,
        //        //    LastName = model.LastName,
        //        //    Email = model.Email,
        //        //    UserName = model.Email,
        //        //};

        //        ApplicationUser users = new ApplicationUser();
        //        users.FirstName = model.FirstName;
        //        users.LastName = model.LastName;
        //        users.Email = model.Email;
        //        users.UserName = model.Email;



        //        var result = await _userManager.CreateAsync(users, model.Password); // Create the user with the provided password

        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(users, "User"); // Assign a default role to the user
        //            return RedirectToAction("Login", "Account");
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description); // Add errors to the model state
        //            }

        //            return View(model);
        //        }
        //    }
        //    return View(model);
        //}
    }
}
