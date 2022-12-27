using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VerifyData2FactorModel
    {
        [Required]
        //The given code from the app
        public string Code { get; set; } = string.Empty;

        //the given returnUrl
        public string returnUrl { get; set; } = string.Empty;

        //bool to remenber the account
        public bool RemenberMe { get; set; }
    }
}
