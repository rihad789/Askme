using QBD.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QBD.Controllers
{
    public class UserController : Controller
    {   
        // GET: Authentication
        public ActionResult login()
        {
            return View();
        }

        public ActionResult signup()
        {
            return View();
        }

        public ActionResult forgot_password()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(login_model login_model)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";

            string email = login_model.email;
            string password =login_model.password;

            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;

            SqlConnection conn = new SqlConnection(connStr);

            SqlCommand cmd = conn.CreateCommand();
            SqlCommand cmd2 = conn.CreateCommand();
            SqlCommand cmd3 = conn.CreateCommand();

            cmd.CommandText = "SELECT Name,Email FROM users where UserID=@userID and Password=@password";
            cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

            cmd2.CommandText = "SELECT COUNT(*) FROM users where UserID=@userID and Password=@password";
            cmd2.Parameters.Add("@userID", SqlDbType.VarChar).Value = email;
            cmd2.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

            cmd3.CommandText = "SELECT COUNT(*) FROM users where UserID=@userID";
            cmd3.Parameters.Add("@userID", SqlDbType.VarChar).Value = email;
            cmd3.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

            SqlDataReader myreader;

            if (ModelState.IsValid == true)
            {
                conn.Open();

                int email_count = (int)cmd3.ExecuteScalar();
                int user_count = (int)cmd2.ExecuteScalar();
                if (email_count==1)
                {
                    if(user_count==1)
                    {
                        try
                        {
                            myreader = cmd.ExecuteReader();
                            while (myreader.Read())
                            {
                                string name = myreader["Name"].ToString();
                                string userID = myreader["Email"].ToString();
                                Session["DisplayName"] = name;
                                Session["userID"] = EncryptString(key, userID);
                            }
                            return RedirectToAction("Index", "Home");

                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("password", "Somethingt is wrong.Please try again");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("password", "Password don;t match");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Email not found.Please register");
                }

            }



            return View();
        }

        [HttpPost]
        public ActionResult signup(signup_model signup_model)
        {
            string userID = signup_model.email;
            string name = signup_model.name;
            string email = signup_model.email;
            string password = signup_model.password;

            string connStr = ConfigurationManager.ConnectionStrings["QBDDBContex"].ConnectionString;

            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "Insert into users (UserID,Email,Password,Name) values (@userID,@email,@password,@name)";
            cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = userID;
            cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
            cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;

            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandText = "SELECT COUNT(*) FROM users where UserID=@userID";
            cmd2.Parameters.Add("@userID", SqlDbType.VarChar).Value = email;

            if (ModelState.IsValid == true)
            {

                try
                {
                    con.Open();
                    int rowsAmount = (int)cmd2.ExecuteScalar();

                    if (rowsAmount == 1)
                    {
                        ModelState.AddModelError("Email", "You are alrerady registered.Please login");
                        
                    }
                    else
                    {
                        cmd.ExecuteNonQuery();
                        con.Close();
                        return RedirectToAction("login");

                    }

                }
                catch (Exception)
                {
                    ModelState.AddModelError("password", "Something is wrong.Please try again");
                }
                
            }

            return View();
        }

        [HttpPost]
        public ActionResult forgot_password(login_model forgot_password)
        {
            if(ModelState.IsValid)
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("ebnaamirfoysal@gmail.com");
                msg.To.Add("rihadnishi@gmail.com");
                msg.Subject = "Recover your Password";
                msg.Body = ("Your Username is:" + "rihad789" + "<br/><br/>" + "Your Password is:" + "password");
                msg.IsBodyHtml = true;

                SmtpClient smt = new SmtpClient();
                smt.Host = "smtp.gmail.com";
                NetworkCredential ntwd = new NetworkCredential();
                ntwd.UserName = "ebnaamirfoysal@gmail.com"; //Your Email ID  
                ntwd.Password = "rr"; // Your Password  
                smt.UseDefaultCredentials = true;
                smt.Credentials = ntwd;
                smt.Port = 587;
                smt.EnableSsl = true;
                smt.Send(msg);
                
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }

        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }


    }

}