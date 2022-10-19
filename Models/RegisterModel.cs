using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "The Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Password is required")]
        [StringLength(50/*MaximunLength*/, MinimumLength = 5, ErrorMessage = "The {0} has to be bigger")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "The ConfirmPassword is required")]
        [Compare("Password", ErrorMessage = "The Password and ConfirmPassword has to be the same")]
        //This is like a If (Compare) can return true or false
        [DataType(DataType.Password)]
        public string ConfirmPassWord { get; set; } = string.Empty;

        //Other Props

        [Required(ErrorMessage = "The Name is required")]
        public string Name { get; set; } = string.Empty;

        public int CountryCode { get; set; }

        public string Telephone { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Country is required")]
        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "The BirthDate is required")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "The State is required")]
        public bool State { get; set; }
    }
}