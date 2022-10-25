using IdentityStart.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace IdentityStart.Services
{
    public interface IAuthService
    {
        Task<UserModel> Register(RegisterModel request);
    }
}
