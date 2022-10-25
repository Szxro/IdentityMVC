using Data;
using IdentityStart.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using System.Reflection.Metadata.Ecma335;

namespace IdentityStart.Services
{
    public class AuthService : IAuthService
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly DataContext _context;
        //Have to inject it (UserManager<IdentityUser>)
        private readonly UserManager<IdentityUser> _manager;
        //Have tp inject it (SignInManager<IdentityUser>)
        private readonly SignInManager<IdentityUser> _signIn;
        public AuthService(IActionContextAccessor actionContextAccessor,DataContext context, UserManager<IdentityUser> manager, SignInManager<IdentityUser> signIn)
        {
            _actionContextAccessor = actionContextAccessor;
            _context = context;
            _manager = manager;
            _signIn = signIn;
        }
        public async Task<UserModel> Register(RegisterModel request)
        {
            //var validation = _actionContextAccessor.ActionContext.ModelState.IsValid;
            //if (validation)
            //{
                //Creating the user if the form is valid
                var user = new UserModel()
                {
                    UserName = request.Name,
                    Name = request.Name,
                    Email = request.Email,
                    CountryCode = request.CountryCode,
                    Telephone = request.Telephone,
                    Country = request.Country,
                    City = request.City,
                    Address = request.Address,
                    BirthDate = request.BirthDate,
                    State = request.State
                };
                //Waiting the UserManager to create it it need the user and the password
                //var result = await _manager.CreateAsync(user, request.Password);

                return user;
            //}
            //return null;
        }
    }
}
