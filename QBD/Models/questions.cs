using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QBD.Models
{
    public class questions
    {
        public string QuestionID { get; set; }
        public string Question { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }
        public string UserID { get; set; }
        public string answer_count { get; set; }
        public string follow { get; set; }

        public string AnswerID { get; set; }
        public string Answer { get; set; }

        public string search_text { get; set; }

        public string userName { get; set; }

        public List<questions> questions_list = new List<questions>();
        public List<questions> questions_list2 = new List<questions>();
        public List<questions> answer_list = new List<questions>();


    }
}