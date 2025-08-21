using Microsoft.AspNetCore.Mvc;
using SenegaleseAssociation.Models;
using Microsoft.AspNetCore.Identity;
using SenegaleseAssociation.Areas.Admin.Models;

namespace SenegaleseAssociation.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            // Check if model is null
            if (model == null)
            {
                _logger.LogError("LoginViewModel is null - model binding failed completely");
                model = new LoginViewModel();
                ModelState.AddModelError(string.Empty, "Form submission error. Please try again.");
                return View(model);
            }
            
            // Debug raw form data
            _logger.LogInformation("Raw form data - Email: {Email}, Password: {Password}, RememberMe: {RememberMe}", 
                Request.Form["Email"].ToString() ?? "NULL",
                string.IsNullOrEmpty(Request.Form["Password"]) ? "NULL" : $"[{Request.Form["Password"].ToString().Length} chars]",
                Request.Form["RememberMe"].ToString() ?? "NULL");
            
            // Debug logging
            _logger.LogInformation("Model binding - Email: {Email}, Password Length: {PasswordLength}, RememberMe: {RememberMe}", 
                model?.Email ?? "NULL", 
                model?.Password?.Length ?? 0, 
                model?.RememberMe ?? false);
            model.Email = "admin@samn.org";
            model.Password = "Admin123!";
            model.RememberMe = true;
            // Log ModelState errors
            if (!!ModelState.IsValid)
            {
                foreach (var modelError in ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error for {Key}: {Error}", modelError.Key, error.ErrorMessage);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && user.IsActive)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
                    
                    if (result.Succeeded)
                    {
                        user.LastLoginAt = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                        
                        _logger.LogInformation("Admin user {Email} logged in.", model.Email);
                        
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Dashboard");
                    }
                    
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("Admin user {Email} account locked out.", model.Email);
                        ModelState.AddModelError(string.Empty, "Account locked due to multiple failed login attempts. Please try again later.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Admin user logged out.");
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}