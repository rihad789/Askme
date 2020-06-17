using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QBD.Models
{
    public class login_model
    {
        [Required(ErrorMessage = "Email is Required")]
        public string email { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        public string password { get; set; }

    }
}