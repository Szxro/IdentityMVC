using Microsoft.AspNetCore.Identity;

namespace IdentityStart.Helper
{
    public interface IHelperService
    {
        void validatingErrors(IdentityResult result);
    }
}
