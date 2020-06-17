using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QBD.Models
{
    public class category
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }

        public string Qcount { get; set; }

        public List<category> category_list = new List<category>();
        public List<category> question_count = new List<category>();

    }
}