using System.Security.Claims;

using MedicalPoint.Data;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Users;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalPoint.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMedicalPointUsersService _medicalPointUsersService;
        private readonly IDegreesService _degreesService;

        public AccountController(IMedicalPointUsersService medicalPointUsersService, IDegreesService degreesService)
        {
            _medicalPointUsersService = medicalPointUsersService;
            _degreesService = degreesService;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Create()
        {
            var degrees = _degreesService.GetAll();
            ViewBag.Degrees = degrees.ConvertAll(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddUserViewModel viewModel, CancellationToken cancellationToken)
        {
            var result = await _medicalPointUsersService.Create(viewModel.Email, viewModel.Password, "SuperAdmin", viewModel.FullName, viewModel.DegreeId, viewModel.MilitaryNumber, viewModel.PhoneNumber, cancellationToken);
            if(!result.Success)
            {
                return View();

            }
            return RedirectToAction("Index","Home");
        }
        public async Task< IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAdmin([FromForm] string email, [FromForm] string password)
        {
            var result = await _medicalPointUsersService.Login(email, password);
            if(!result.Success)
            {
                return View();
            }
            var user = result.Data;
            if(user.AccoutType != "")
            {
                //return RedirectToAction(nameof(AccessDenied));
            }
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.AccoutType));
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            var userfromcookieE = HttpContext.User;
            return RedirectToAction("Index", "Home");

        }
    }
}
