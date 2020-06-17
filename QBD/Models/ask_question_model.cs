using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QBD.Models
{
    public class ask_question_model
    {

        [Required(ErrorMessage = "Category Must be selected")]
        public string question_Category { get; set; }

        [Required(ErrorMessage = "Question feild can not be empty")]
        public string question { get; set; }

    }
}