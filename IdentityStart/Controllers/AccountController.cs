using IdentityStart.Helper;
using IdentityStart.Models;
using IdentityStart.Services;
using IdentityStart.Services.UrlService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace IdentityStart.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _service;
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _manager;
        private readonly IHelperService _helper;
        private readonly IEmailSender _mail;
        private readonly IUrlServices _url;
        public AccountController(
            IAuthService service,
            SignInManager<IdentityUser> signIn, 
            UserManager<IdentityUser> manager,
            IHelperService helper,
            IEmailSender mail,
            IUrlServices url
            )
        {
            _service = service;
            _signIn = signIn;
            _manager = manager;
            _helper = helper;
            _mail = mail;
            _url = url;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
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
                    var emailConfirmationCode = await _manager.GenerateEmailConfirmationTokenAsync(user);
                    var url = _url.generateURL("Account", "ConfirmEmail", emailConfirmationCode,user.Id);
                    await _mail.SendEmailAsync(request.Email, "Confirm your Email",
                   "Click this link to confirmt your email:<a href=\"" + url + "\">Link</a>"
                   );


                    if (!user.EmailConfirmed)
                    {
                        return View("ConfirmEmail");
                    }
                    await _signIn.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("/Home/Index");
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AccessModel request, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var result = await _signIn.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe,lockoutOnFailure:true);

                if (result.Succeeded)
                {
                    return LocalRedirect("/Home/Privacy");
                }
                else if (result.IsLockedOut)
                {
                    return LocalRedirect("/Account/Locked");
                }
                else if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(Verify2FactorCode), new{returnUrl = returnurl,remeberMe = request.RememberMe });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The UserName or Password are incorrect");
                    return View(request);
                }
            }

                return View(request);
        }

        //Lockout View
        [HttpGet]
        public IActionResult Locked()
        {
            return View();
        }


        //Logout Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return LocalRedirect("/Home/Index");
        }

        //ForgetPassword View
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

       // Forget Password Functionality
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPaswordModel request)
        {
            if (ModelState.IsValid)
            {
                //Finding the user by the email
                var user = await _manager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid User");
                    return View(request);
                }

                var code = await _manager.GeneratePasswordResetTokenAsync(user);
                var url = _url.generateURL("Account","ResetPassword",code);

                 await _mail.SendEmailAsync(request.Email, "Reset Your Password",
                    "Click this link to reset your password:<a href=\"" + url + "\">Link</a>"
                    );
                return RedirectToAction("ConfirmPassword");   
            }
            return View(request);
        }

        //Confirm Password
        [HttpGet]

        public IActionResult ConfirmPassword()
        {
            return View();
        }

        //Reset Password
        [HttpGet]
        public IActionResult ResetPassword(string code)
        {
            return code == null ? View("ErrorView") : View();
        }

        [HttpPost]

        public async Task<IActionResult> ResetPassword(ResetPasswordModel request) 
        {
            if (ModelState.IsValid)
            {
                var email = await _manager.FindByEmailAsync(request.Email);
                if (email == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email");
                    return View(request);
                }

                var resetPassword = await _manager.ResetPasswordAsync(email,request.Code,request.Password);

                if (resetPassword.Succeeded)
                {
                    return RedirectToAction("SuccessResetPassword");
                }

                _helper.validatingErrors(resetPassword);
            }

            return View(request);
        }

        // SuccessResetPassword View
        [HttpGet]

        public IActionResult SuccessResetPassword()
        {
            return View();
        }

        //ConfirmEmail View and functionality

        [HttpGet]

        public async Task<IActionResult> ConfirmEmail(string uid,string code)
        {
            if (uid == null || code == null)
            {
                return View("ErrorView");
            }

            var user = await _manager.FindByIdAsync(uid);
            if (user == null || user.EmailConfirmed == true)
            {
                return View("ErrorView");
            }

            var result = await _manager.ConfirmEmailAsync(user,code);

            return View(result.Succeeded ? "ConfirmEmail" : "ErrorView");
        }


        //Auth Outsiders

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult AuthOutsider(string provider,string returnUrl = null)
        {
            var urlCallback = _url.generateURL("Account","AuthOutsiderCallback","","",returnUrl);
            var propirties = _signIn.ConfigureExternalAuthenticationProperties(provider, urlCallback);
            return Challenge(propirties,provider);
        }

        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> AuthOutsiderCallback(string returnUrl = null,string error = null)
        {
            //The url by default is going to be ("/")
            returnUrl = returnUrl ?? Url.Content("~/");

            if (error != null)
            {
                ModelState.AddModelError(string.Empty, $"Error was found ${error}");
                return View(nameof(Login));
            }

            //going to get some info about the user (like name,email,etc..)
            var info = await _signIn.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _signIn.ExternalLoginSignInAsync(info.LoginProvider,info.ProviderKey,isPersistent:false);
            if (result.Succeeded)
            {
                //Update the access tokens
                await _signIn.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor) 
            {
                return RedirectToAction(nameof(Verify2FactorCode) ,new {returnUrl = returnUrl});
            }
            else
            {
                ViewData["returnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.ProviderDisplayName;
                //Getting the name and the email (in the claims are the provider Key,the name and the email)
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                //Sedding to another view with the email and name to save it in the DB
                return View(nameof(AuthOutsiderResult), new AuthOutsider { Email = email, Name = name });
            }
        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> AuthOutsiderResult(AuthOutsider request,string returnUrl = null)
        {
            //path by default "/"
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //Getting the loginProvider and the providerKey
                var info = await _signIn.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ErrorView");
                }

                //Creating the user with the given data
                var user = new IdentityUser() {UserName = request.Name,Email = request.Email};
                var result = await _manager.CreateAsync(user);

                if (result.Succeeded)
                {
                    //Verify the external login and the add the login
                    result = await _manager.AddLoginAsync(user,info);  
                    if(result.Succeeded)
                    {
                        //log the user by default
                        await _signIn.SignInAsync(user,isPersistent:false);
                        //update the access token
                        await _signIn.UpdateExternalAuthenticationTokensAsync(info);
                        return LocalRedirect(returnUrl);
                    }
                }
                //Putting the errors in the view
                _helper.validatingErrors(result);
            }
            //If the modelState is invalid is going to return to same url and put the errors
            ViewData["returnUrl"] = returnUrl;
            return View(request);
        }

        //Two Factor Auth Get

        [HttpGet]

        public async Task<IActionResult> ActivateTwoFactor()
        {
            //Obtaining the user by the claims
            var user = await _manager.GetUserAsync(User);
            //reseting the auth key
            await _manager.ResetAuthenticatorKeyAsync(user);
            //gettting the token for the 2FAUTH
            var token = await _manager.GetAuthenticatorKeyAsync(user);

            var twoAuth = new _2FAUTHModel() { Token = token };

            return View(twoAuth);
        }


        //Enable the Two Factor Auth Post

        [HttpPost]
        public async Task<IActionResult> ActivateTwoFactor(_2FAUTHModel request)
        {
            if (ModelState.IsValid)
            {
                //Getting the user
                var user = await _manager.GetUserAsync(User);
                //Getting the tokenProvider
                var tokenProvider = _manager.Options.Tokens.AuthenticatorTokenProvider;
                //bol to verify the code given
                var success = await _manager.VerifyTwoFactorTokenAsync(user, tokenProvider, request.Code);

                if (success)
                {
                    //enable the 2FACTOR
                    await _manager.SetTwoFactorEnabledAsync(user, true);
                }

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "The Two Factor Auth is invalid");
                    return View(request);
                }
            }

            return View(nameof(ConfirmTwoFactor));
        
        }

        //ConfirmationTwoFactor View

        [HttpGet]

        public IActionResult ConfirmTwoFactor()
        {
            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Verify2FactorCode(bool remenberMe,string returnUrl = null)
        {
            //Getting the user with the TwoFactor
            var user = await _signIn.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("ErrorView");
            }
            //When the user is going to redirect after completing the fields

            ViewData["returnUrl"] = returnUrl;
            //the code have the user give it
            return View(new VerifyData2FactorModel() {returnUrl = returnUrl, RemenberMe = remenberMe });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Verify2FactorCode(VerifyData2FactorModel request)
        {
            request.returnUrl = Url.Content("~/");
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            //Verificating the code and given a reesponse
            var result = await _signIn.TwoFactorAuthenticatorSignInAsync(request.Code,request.RemenberMe,rememberClient:true);

            if (result.Succeeded)
            {
                return LocalRedirect(request.returnUrl);
            }

            if (result.IsLockedOut)
            {
                return View("Locked");
            }

            if (!result.Succeeded) 
            {
                ModelState.AddModelError(string.Empty, "Invalid Code");
                return View(request);
            }

            return View(request);
            
        }
    }
}
