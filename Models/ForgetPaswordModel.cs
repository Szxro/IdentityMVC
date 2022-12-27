using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ForgetPaswordModel
    {
        [Required(ErrorMessage = "The Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
