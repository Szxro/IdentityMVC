using Microsoft.AspNetCore.Identity;

namespace IdentityStart.Services.UrlService
{
    public interface IUrlServices
    {
        string generateURL(string controller,string method,string code = "" ,string uid = "",string returnUrl = "");
    }
}
