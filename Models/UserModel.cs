//using IdentityStart.Models.Common;
//using Microsoft.AspNet.Identity.EntityFramework;


using Microsoft.AspNetCore.Identity;

namespace IdentityStart.Models
{
    public class UserModel :  IdentityUser
    {
        //This Model is going to be added to the AspUser table
        public string Name { get; set; } = string.Empty;

        public int CountryCode { get; set; }

        public string Telephone { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
        
        public bool State { get; set; }
    }
}
