using QBD.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace QBD.Controllers
{
     
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            category();
            return View();
        }

        public ActionResult ask_questions()
        {
            category();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult questions()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult saved_question()
        {

            return View();
        }

        public ActionResult search_question()
        {
            category();
            return View();
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
                questions q_list = new questions();
                category();
                string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;
                SqlConnection con = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand("select question.QuestionID,question.Question,question.answer_count,question.follow ,users.Name from question inner join users on question.UserID=users.UserID");
                cmd.Connection = con;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<questions> d_questions_list = new List<questions>();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        var questions_list = new questions();
                        questions_list.QuestionID = sdr["QuestionID"].ToString();
                        questions_list.Question = sdr["Question"].ToString();
                        questions_list.answer_count = sdr["answer_count"].ToString();
                        questions_list.follow = sdr["follow"].ToString();
                        questions_list.userName = sdr["Name"].ToString();
                        d_questions_list.Add(questions_list);

                    }

                    q_list.questions_list = d_questions_list;            
                }

            con.Close();

            return View(q_list);

        }

        [HttpPost]
        public ActionResult ask_questions(string category_id, string question_text, string userID)
        {
            category();
            var key = "b14ca5898a4e4133bbce2ea2315a1916";

            string question_ID;
            if (question_text.Length > 150)
            { question_ID = question_text.Substring(0, 150); }
            else { question_ID = question_text; }
            question_ID = question_ID.Replace(" ", "_");

            string status = "";
            string date = DateTime.Now.ToString("yyyy/MM/d");

            userID = DecryptString(key, userID);


            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "Insert into question (QuestionID,Question,Category,Date,UserID,answer_count,follow) values (@QuestionID,@question,@category,@date,@userID,@answer_count,@follow)";
            cmd.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = question_ID;
            cmd.Parameters.Add("@question", SqlDbType.VarChar).Value = question_text;
            cmd.Parameters.Add("@category", SqlDbType.VarChar).Value = category_id;
            cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
            cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = userID;
            cmd.Parameters.Add("@answer_count", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@follow", SqlDbType.Int).Value = 0;

            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandText = "update category set Qcount = Qcount + 1  where categoryID = @category ";
            cmd2.Parameters.Add("@category", SqlDbType.Int).Value = category_id;


            try
            {
                con.Open();
                if (cmd.ExecuteNonQuery()>0)
                {
                    if (cmd2.ExecuteNonQuery() > 0)
                    {
                        status = "Question submitted.Care to ask another?";
                    }
                }
                con.Close();
               

            }
            catch (Exception)
            {
                status = "Sorry! Unable to submit question.";
            }

            return Content(status);
        }
      
        [HttpGet]
        public ActionResult questions(string questionID)
        {
            questions q_list = new questions();
            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;

            if (string.IsNullOrEmpty(questionID))
            {
                category();
                
                SqlConnection con = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand("select question.QuestionID,question.Question,question.answer_count,question.follow ,users.Name from question inner join users on question.UserID=users.UserID");
                cmd.Connection = con;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<questions> d_questions_list = new List<questions>();

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        var questions_list = new questions();
                        questions_list.QuestionID = sdr["QuestionID"].ToString();
                        questions_list.Question = sdr["Question"].ToString();
                        questions_list.answer_count = sdr["answer_count"].ToString();
                        questions_list.follow = sdr["follow"].ToString();
                        questions_list.userName = sdr["Name"].ToString();
                        d_questions_list.Add(questions_list);

                    }

                    q_list.questions_list = d_questions_list;
                    
                }
                con.Close();

            }
            else
            {
                category();
                ViewBag.questionID = questionID;
                SqlConnection con = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand("select question.QuestionID,question.Category,question.Question,question.answer_count,question.follow ,users.Name from question  inner join users on question.UserID=users.UserID  and question.QuestionID=@QuestionID");
                cmd.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = questionID;
                cmd.Connection = con;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<questions> questions_list = new List<questions>();
                string categoryID = "";

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        var questions = new questions();
                        questions.QuestionID = sdr["QuestionID"].ToString();
                        questions.Question = sdr["Question"].ToString();
                        questions.answer_count = sdr["answer_count"].ToString();
                        categoryID= sdr["Category"].ToString();
                        questions.follow = sdr["follow"].ToString();
                        questions.userName = sdr["Name"].ToString();
                        questions_list.Add(questions);

                    }

                    q_list.questions_list = questions_list;
                    
                }
                con.Close();

                SqlCommand cmd2 = new SqlCommand("select answer.AnswerID,answer.Answer,users.Name from answer inner join answer_details on answer.AnswerID=answer_details.AnswerID and QuestionID=@QuestionID inner join users on answer.UserID=users.UserID");
                cmd2.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = questionID;
                cmd2.Connection = con;
                con.Open();
                SqlDataReader sdr2 = cmd2.ExecuteReader();
                List<questions> answer = new List<questions>();
                

                if (sdr2.HasRows)
                {
                    while (sdr2.Read())
                    {
                        var answer_list = new questions();
                        answer_list.AnswerID = sdr2["AnswerID"].ToString();
                        answer_list.Answer = sdr2["Answer"].ToString();
                        answer_list.userName= sdr2["Name"].ToString();
                        answer.Add(answer_list);
                    }

                    q_list.answer_list= answer;

                    
                }

                con.Close();

                SqlCommand cmd3 = new SqlCommand("select question.QuestionID,question.Category,question.Question,question.answer_count,question.follow ,users.Name from question  inner join users on question.UserID=users.UserID  and question.Category=@Category and question.QuestionID!=@QuestionID");
                cmd3.Parameters.Add("@Category", SqlDbType.Int).Value = categoryID;
                cmd3.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = questionID;
                cmd3.Connection = con;
                con.Open();
                SqlDataReader sdr3 = cmd3.ExecuteReader();
                List<questions> questions_list2 = new List<questions>();

                if (sdr3.HasRows)
                {
                    while (sdr3.Read())
                    {
                        var questions = new questions();
                        questions.QuestionID = sdr3["QuestionID"].ToString();
                        questions.Question = sdr3["Question"].ToString();
                        questions.answer_count = sdr3["answer_count"].ToString();
                        categoryID = sdr3["Category"].ToString();
                        questions.follow = sdr3["follow"].ToString();
                        questions.userName = sdr3["Name"].ToString();
                        questions_list2.Add(questions);

                    }

                    q_list.questions_list2 = questions_list2;
                    
                }

                con.Close();



            }
            return View(q_list);
        }

        [HttpGet]
        public ActionResult followed_question(string userID)
        {
            if (!String.IsNullOrEmpty(userID))
            {
                var key = "b14ca5898a4e4133bbce2ea2315a1916";

                userID = DecryptString(key, userID);

                questions q_list = new questions();
                category();

                string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;
                SqlConnection con = new SqlConnection(connStr);

                SqlCommand cmd = new SqlCommand("select question.QuestionID,question.Question,question.answer_count,question.follow ,users.Name from question inner join save_question on question.QuestionID = save_question.QuestionID and save_question.UserID = @UserID inner join users on users.UserID = @UserID");
                cmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = userID;
                cmd.Connection = con;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<questions> questions_list = new List<questions>();

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        var questions = new questions();
                        questions.QuestionID = sdr["QuestionID"].ToString();
                        questions.Question = sdr["Question"].ToString();
                        questions.answer_count = sdr["answer_count"].ToString();
                        questions.follow = sdr["follow"].ToString();
                        questions.userName = sdr["Name"].ToString();
                        questions_list.Add(questions);

                    }

                    q_list.questions_list = questions_list;
                    con.Close();
                }
                return View(q_list);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult follow_questions(string userID,string questionID)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";

            userID = DecryptString(key, userID); 
            string status = "";
            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "insert into save_question(QuestionID,UserID)values(@QuestionID,@UserID)";
            cmd.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = questionID;
            cmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = userID;

            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandText = "update  Question set follow=follow+1 where QuestionID=@QuestionID";
            cmd2.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = questionID;

            SqlCommand cmd3 = con.CreateCommand();
            cmd3.CommandText = "select COUNT(*) from  save_question where QuestionID=@QuestionID and UserID=@UserID";
            cmd3.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = questionID;
            cmd3.Parameters.Add("@UserID", SqlDbType.VarChar).Value = userID;


            if (string.IsNullOrEmpty(userID))
            {
                status = "You have be logged in to save question";
            }
            else
            {
                try
                {
                    con.Open();
                    int rowsAmount = (int)cmd3.ExecuteScalar();
                    if(rowsAmount==1)
                    {
                        status = "You already saved this question";
                    }
                    else
                    {
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            if (cmd2.ExecuteNonQuery() > 0)
                            { status = "This question is save to your account"; }
                        }
                        else
                        { status = "Something is wrong "; }
                    }


                    con.Close();
                }
                catch (Exception)
                {  status = "There was a problem saving your question";  }
            }

            return Content(status);

        }

        [HttpGet]
        public ActionResult search_question(string search_text)
        {
                category();
                questions q_list = new questions();
                string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;
                
                SqlConnection con = new SqlConnection(connStr);
                SqlCommand cmd = con.CreateCommand();
                
                cmd.CommandText = "SELECT question.QuestionID,question.Question,question.answer_count,question.follow,users.Name FROM question inner join users on question.UserID = users.UserID and Question LIKE '%" + search_text + "%'";
                
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<questions> d_questions_list = new List<questions>();

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        var questions_list = new questions();
                        questions_list.QuestionID = sdr["QuestionID"].ToString();
                        questions_list.Question = sdr["Question"].ToString();
                        questions_list.answer_count = sdr["answer_count"].ToString();
                        questions_list.follow = sdr["follow"].ToString();
                        questions_list.userName = sdr["Name"].ToString();
                        d_questions_list.Add(questions_list);

                    }

                    q_list.questions_list = d_questions_list;
                    con.Close();
                }
                return View(q_list);
        }

        [HttpGet]
        public ActionResult category(string categoryID)
        {
            questions q_list = new questions();
            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;

            if (string.IsNullOrEmpty(categoryID))
            {
                category();

                SqlConnection con = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand("select question.QuestionID,question.Question,question.answer_count,question.follow ,users.Name from question inner join users on question.UserID=users.UserID");
                cmd.Connection = con;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<questions> d_questions_list = new List<questions>();

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        var questions_list = new questions();
                        questions_list.QuestionID = sdr["QuestionID"].ToString();
                        questions_list.Question = sdr["Question"].ToString();
                        questions_list.answer_count = sdr["answer_count"].ToString();
                        questions_list.follow = sdr["follow"].ToString();
                        questions_list.userName = sdr["Name"].ToString();
                        d_questions_list.Add(questions_list);


                    }

                    q_list.questions_list = d_questions_list;
                    con.Close();
                }

            }
            else
            {
                category();
                ViewBag.questionID = categoryID;
                SqlConnection con = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand("select question.QuestionID,question.Question,question.answer_count,question.follow ,users.Name from question inner join users on question.UserID=users.UserID and question.Category=@Category");
                cmd.Parameters.Add("@Category", SqlDbType.Int).Value = categoryID;
                cmd.Connection = con;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<questions> questions_list = new List<questions>();

                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        var questions = new questions();
                        questions.QuestionID = sdr["QuestionID"].ToString();
                        questions.Question = sdr["Question"].ToString();
                        questions.answer_count = sdr["answer_count"].ToString();
                        questions.follow = sdr["follow"].ToString();
                        questions.userName= sdr["Name"].ToString(); ;
                        questions_list.Add(questions);

                    }

                    q_list.questions_list = questions_list;
                    con.Close();
                }

            }
            return View(q_list);
        }

        [HttpPost]
        public ActionResult answer_questions(string answer_text, string userID,string QuestionID)
        {
            string answer_ID;
            if(answer_text.Length>150)
            { answer_ID = answer_text.Substring(0, 150); }
            else { answer_ID = answer_text; }

            string status = "";
            string answerID = answer_ID.Replace(" ", "_");
            answer_text = Regex.Replace(answer_text, "((https?://)?www\\.[^\\s]+)", "<a href=\"$1\" target=\"_blank\">$1</a>");
            answer_text = answer_text.Replace("\r", "<br>").Replace("\n", "<br>");

            answer_text = "<p>" + answer_text + "</p>";
            string date = DateTime.Now.ToString("yyyy/MM/d");

            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            userID = DecryptString(key, userID);

            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "insert into answer(AnswerID,Answer,Date,UserID)values(@AnswerID,@Answer,@Date,@UserID)";
            cmd.Parameters.Add("@AnswerID", SqlDbType.VarChar).Value = answerID;
            cmd.Parameters.Add("@Answer", SqlDbType.VarChar).Value = answer_text;
            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = date;
            cmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = userID;

            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandText = "update  Question set answer_count=answer_count+1 where QuestionID=@QuestionID";
            cmd2.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = QuestionID;

            SqlCommand cmd3 = con.CreateCommand();
            cmd3.CommandText = "insert into answer_details(QuestionID,AnswerID)values(@QuestionID,@AnswerID)";
            cmd3.Parameters.Add("@QuestionID", SqlDbType.VarChar).Value = QuestionID;
            cmd3.Parameters.Add("@AnswerID", SqlDbType.VarChar).Value = answerID;

            try
            {
                con.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    if (cmd2.ExecuteNonQuery() > 0)
                    {
                        cmd3.ExecuteNonQuery();
                    }
                    else { status = "Sorry! Couldn't update answer Count"; }
                }
                else { status = "Sorry! Unable to submit answer."; }
                con.Close();
                status = "Thanks for your answer";

            }
            catch (Exception)
            {
                status = "Sorry!Unable to submit answer.";
            }

            return Content(status);
        }

        public ActionResult question_count()
        {
            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("select * from category");
            cmd.Connection = con;
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            List<category> category_count = new List<category>();
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    var category = new category();
                    category.CategoryID = sdr["categoryID"].ToString();
                    category.CategoryName = sdr["categoryName"].ToString();
                    category.Qcount = sdr["Qcount"].ToString();
                    category_count.Add(category);

                }
                con.Close();
            }
            return Json(category_count, JsonRequestBehavior.AllowGet);
        }

        public void category()
        {

            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("select * from category");
            cmd.Connection = con;
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            List<category> category_list = new List<category>();
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    var category = new category();
                    category.CategoryID = sdr["categoryID"].ToString();
                    category.CategoryName = sdr["categoryName"].ToString();
                    category.Qcount = sdr["Qcount"].ToString();
                    category_list.Add(category);

                }
                con.Close();

                ViewBag.category_list = category_list;
            }
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}


