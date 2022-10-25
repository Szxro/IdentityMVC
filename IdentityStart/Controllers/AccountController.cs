using IdentityStart.Helper;
using IdentityStart.Models;
using IdentityStart.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Models;

namespace IdentityStart.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _service;
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _manager;
        private readonly IHelperService _helper;
        public AccountController(IAuthService service, SignInManager<IdentityUser> signIn, UserManager<IdentityUser> manager,IHelperService helper)
        {
            _service = service;
            _signIn = signIn;
            _manager = manager;
            _helper = helper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var register = new RegisterModel() { };
            return View(register);
        }

        //Saving the user

        [HttpPost]

        public async Task<IActionResult> Register(RegisterModel request)
        {
            if (ModelState.IsValid)
            {
                var user = await _service.Register(request);
                //In this part the User is Saved
                var manager = await _manager.CreateAsync(user, request.Password);
                if (manager.Succeeded)
                {
                    await _signIn.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _helper.validatingErrors(manager);
                    return View(request);
                }
            }

            return View(request); 
        }

        //Login Part
        [HttpGet]

        public ActionResult Login()
        {
            var login = new AccessModel() { };
            return View(login);
        }

        //Login Function

        [HttpPost]
        public async Task<ActionResult> Login(AccessModel request)
        {
            if (ModelState.IsValid)
            {
                var result = await _signIn.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe,lockoutOnFailure:false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else 
                {
                    ModelState.AddModelError(string.Empty, "The UserName or Password are incorrect");
                    return View(request);
                }
            }

                return View(request);
        }

        //Logout Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Home");   
        }
    }
}
