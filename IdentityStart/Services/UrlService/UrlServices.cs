using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace IdentityStart.Services.UrlService
{
    public class UrlServices : IUrlServices
    {
        private readonly UserManager<IdentityUser> _manager;
        private readonly IHttpContextAccessor _http;
        private readonly IUrlHelper _url;

        public UrlServices(
            UserManager<IdentityUser> manager,
            IHttpContextAccessor http,
            IUrlHelper url
            )
        {
            _manager = manager;
            _http = http;
            _url = url; 
        }
        public string generateURL(string controller,string method,string code = "",string uid = "",string returnUrl = "")
        {
            //var code = await _manager.GeneratePasswordResetTokenAsync(user);

            //var email_body =
            //    "Please confirm to change the password address <a href =\"#URL#\">Click here</a>";

            var callback_url = "";

            if (uid != "")
            {
                callback_url = _http.HttpContext.Request.Scheme + "://" + _http.HttpContext.Request.Host
                      + _url.Action(method, controller, new { code = code, uid = uid });
            }
            else if (returnUrl != "")
            {
                callback_url = _http.HttpContext.Request.Scheme + "://" + _http.HttpContext.Request.Host
                      + _url.Action(method, controller, new { returnUrl = returnUrl });
            }
            else if (code != "")
            {
                callback_url = _http.HttpContext.Request.Scheme + "://" + _http.HttpContext.Request.Host
                      + _url.Action(method, controller, new { code = code });
            }
            else 
            {
                callback_url = _http.HttpContext.Request.Scheme + "://" + _http.HttpContext.Request.Host
                      + _url.Action(method, controller);
            }
            //var new_email_body = email_body.Replace("#URL#", callback_url);

            return callback_url;
        }
    }
}
