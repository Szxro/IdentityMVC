using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AccessModel 
    {
        [Required(ErrorMessage ="The Username is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Password is required")]
        [StringLength(50/*MaximunLength*/, MinimumLength = 5, ErrorMessage = "The {0} has to be bigger")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
