using System.Security.Claims;

using MedicalPoint.Constants;
using MedicalPoint.Data;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Users;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.User.Claims.FirstOrDefault(x=> x.Type == ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Index","Home");
            }
            var userId = int.Parse(userIdString);
            var user = await _medicalPointUsersService.Get(userId);
            var userViewMode = new UserViewModel
            {
                AccountType = user.AccoutType,
                Email = user.Email,
                Id = user.Id,
                MilitaryNumber = user.MilitaryNumber,
                Name = user.FullName,
                PhoneNumber = user.PhoneNumber,
                DegreeName = user.Degree.Name,
            };
         
            return View(userViewMode);
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult LoginRegist()
        {
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Patients");
            }
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
            ViewBag.AccountTypes = ConstantUserType.Types.Select(x => new SelectListItem
            {
                Text = x,
                Value = x
            });
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddUserViewModel viewModel, CancellationToken cancellationToken)
        {
            var result = await _medicalPointUsersService.Create(viewModel.Email, viewModel.Password, viewModel.AccountType, viewModel.FullName, viewModel.DegreeId, viewModel.MilitaryNumber, viewModel.PhoneNumber, cancellationToken);
            if(!result.Success)
            {
                return View();

            }
            return RedirectToAction("Index","Home");
        }
        public async Task< IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        //}
        //private async Task<bool> LoginUser(string email, string password, string accountType)
        //{
        //    var result = await _medicalPointUsersService.Login(email, password);
        //    if (!result.Success)
        //    {
        //        return false;
        //    }
        //    var user = result.Data;
        //    if (user.AccoutType != accountType)
        //    {
        //        return false;
        //        //return RedirectToAction(nameof(AccessDenied));
        //    }
        //    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        //    identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
        //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        //    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        //    identity.AddClaim(new Claim(ClaimTypes.Role, user.AccoutType));
        //    var authProperties = new AuthenticationProperties
        //    {
        //        AllowRefresh = true,
        //        // Refreshing the authentication session should be allowed.

        //        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
        //        // The time at which the authentication ticket expires. A 
        //        // value set here overrides the ExpireTimeSpan option of 
        //        // CookieAuthenticationOptions set with AddCookie.


        //        // Whether the authentication session is persisted across 
        //        // multiple requests. When used with cookies, controls
        //        // whether the cookie's lifetime is absolute (matching the
        //        // lifetime of the authentication ticket) or session-based.

        //        //IssuedUtc = <DateTimeOffset>,
        //        // The time at which the authentication ticket was issued.

        //        //RedirectUri = <string>
        //        // The full path or absolute URI to be used as an http 
        //        // redirect response value.
        //    };
        //    var principal = new ClaimsPrincipal(identity);
        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        //    return true;
        //}

        public async Task<IActionResult> Logindoctor([FromForm] string email, [FromForm] string password)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Visits");
            }
            var result = await _medicalPointUsersService.Login(email, password);
            if (!result.Success)
            {
                return View();
            }
            var user = result.Data;
            if (user.AccoutType != "")
            {
                //return RedirectToAction(nameof(AccessDenied));
            }
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.AccoutType));
            
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var userfromcookieE = HttpContext.User;
            return RedirectToAction("Index", "Visits");

        }

        public async Task<IActionResult> LoginPharmacy([FromForm] string email, [FromForm] string password)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Medicines");
            }
            var result = await _medicalPointUsersService.Login(email, password);
            if (!result.Success)
            {
                return View();
            }
            var user = result.Data;
            if (user.AccoutType != "")
            {
                //return RedirectToAction(nameof(AccessDenied));
            }
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.AccoutType));
           
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var userfromcookieE = HttpContext.User;
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> LoginSuperAdmin([FromForm] string email, [FromForm] string password)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin");
            }
            var result = await _medicalPointUsersService.Login(email, password);
            if (!result.Success)
            {
                return View();
            }
            var user = result.Data;
            if (user.AccoutType != "")
            {
                //return RedirectToAction(nameof(AccessDenied));
            }
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.AccoutType));
            
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var userfromcookieE = HttpContext.User;
            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        public async Task<IActionResult> LoginRegist([FromForm] string email, [FromForm] string password)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Patients");
            }
            var result = await _medicalPointUsersService.Login(email, password);
            if (!result.Success)
            {
                return View();
            }
            var user = result.Data;
            if (user.AccoutType != "")
            {
                //return RedirectToAction(nameof(AccessDenied));
            }
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.AccoutType));
           
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var userfromcookieE = HttpContext.User;
            return RedirectToAction("Index", "Patients");

        }
        public async Task<IActionResult> LoginAdmin([FromForm] string email, [FromForm] string password)
        {
            var result = await _medicalPointUsersService.Login(email, password);
            if (!result.Success)
            {
                return View();
            }
            var user = result.Data;
            if (user.AccoutType != "")
            {
                //return RedirectToAction(nameof(AccessDenied));
            }
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.AccoutType));
            
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var userfromcookieE = HttpContext.User;
            return RedirectToAction("Index", "Home");

        }
    }
}
