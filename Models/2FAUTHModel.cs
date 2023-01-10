using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class _2FAUTHModel
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        //Token for activate the 2FAUTH
        public string Token { get; set; } = string.Empty;

        //For the qr Code
        public string AuthCode { get; set; } = string.Empty;
    }
}
