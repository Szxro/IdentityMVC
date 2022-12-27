using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "The Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Password is required")]
        [StringLength(50/*MaximunLength*/, MinimumLength = 5, ErrorMessage = "The {0} has to be bigger")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Confirmation Password is required")]
        [Compare("Password",ErrorMessage = "The Password & Confirm Password have to be the same")]
        [DataType(DataType.Password)]
        public string ConfirmationPassword { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }
}
