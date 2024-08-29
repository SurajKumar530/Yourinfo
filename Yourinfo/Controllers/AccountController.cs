using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Yourinfo.Models;
using Yourinfo.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Yourinfo.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminRepository _admin;
        private readonly IUserRepository _user;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<AdminController> _logger;
        public AccountController(IAdminRepository admin, IWebHostEnvironment hostingEnvironment, ILogger<AdminController> logger, IUserRepository user)
        {
            _admin = admin;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _user = user;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous] // Allows access without authentication
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous] // Allows access without authentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (model.Username == null && model.Password == null)
                {
                    return BadRequest("Invalid Request ");
                }

                User user = await _admin.AuthenticateUser(model.Username, model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        // Add more claims as needed
                    };

                    // Add roles as claims
                    foreach (var role in user.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        RedirectUri = returnUrl,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                        AllowRefresh = true
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    await LogUserLoginHistoryAsync(user.Id);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());
                        HttpContext.Session.SetString("Username", model.Username);
                        // Determine the redirect URL based on user roles
                        if (user.Roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (user.Roles.Contains("Customer"))
                        {
                            return RedirectToAction("Index", "Customer");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Securitybreach()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Forget()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Forget(string email)
        {
            try
            {
                var getUserExist = await _admin.getUser(email);
                if (getUserExist > 0)
                {
                    return RedirectToAction("Reset", "Admin", new { Email = email });
                }
                else
                {
                    return BadRequest("Invalid Email . Email Not Exist !");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Reset(string Email)
        {
            LoginViewModel model = new LoginViewModel();
            model.email = Email;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Reset(LoginViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Model is null");
                }
                var resetResult = await _admin.ResetPassword(model.email, model.Password);
                if (resetResult > 0)
                {
                    return RedirectToAction("SuccessPassword", "Admin");
                }
                else
                {
                    return BadRequest("Error While Reset Password !");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IActionResult SuccessPassword()
        {
            return View();
        }

        private async Task LogUserLoginHistoryAsync(int userId)
        {
            var expiresUtc = DateTime.UtcNow.AddHours(1);
            var localExpiresTime = expiresUtc.ToLocalTime();
            var deviceIP = HttpContext.Connection.RemoteIpAddress.ToString();
            var deviceName = Request.Headers["User-Agent"].ToString();
            var browserInfo = Request.Headers["User-Agent"].ToString();
            var loginStatus = "Success";
            var sessionToken = Guid.NewGuid().ToString();

            await _user.LogUserLoginHistoryAsync(userId, DateTime.Now, localExpiresTime, deviceIP, deviceName, browserInfo, loginStatus, sessionToken);
        }

    }
}
