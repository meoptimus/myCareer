using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{

    public class AccountModel
    {
        myCareerEntities db = new myCareerEntities();
        private List<user> listAccounts = new List<user>();
        public AccountModel()
        {
            try
            {
                listAccounts = db.users.Where(a => a.status.Equals("ACTIVE")).ToList();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                HttpContext.Current.Response.Write("<script> alert('" + error + ");</script>");
            }
        }

        public user find( string username)
        {
            return listAccounts.Where(a => a.user_name.Equals(username)).FirstOrDefault();
        }

        public user login(string username, string password)
        {
            return listAccounts.Where(a => a.user_name.Equals(username) && a.password.Equals(password)).FirstOrDefault();
        }

    }
}