using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace IdentityStart.Helper
{
    public class HelperService : IHelperService
    {
        private readonly IActionContextAccessor _action;

        public HelperService(IActionContextAccessor action)
        {
            _action = action;
        }
    
        public void validatingErrors(IdentityResult result)
        {
            var model = _action.ActionContext.ModelState;
            foreach (var i in result.Errors)
            {
                model.AddModelError(string.Empty,i.Description);
            }
        }
    }
}
