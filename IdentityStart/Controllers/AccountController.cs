using Microsoft.AspNetCore.Mvc;
using Models;

namespace IdentityStart.Controllers
{
    public class AccountController : Controller
    {
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
    }
}
