using myCareer.Models;
using myCareer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myCareer.Controllers
{
    public class LoginController : Controller
    {
        myCareerEntities db = new myCareerEntities();

        // GET: Login
        public ActionResult Index(string returnUrl)
        {
           
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                ViewBag.returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);
            return View("login");
        }
        [HttpPost]
        public ActionResult loginVerify(string username = "", string password = "", string returnurl = "")
        {
            Md5Calculator Md5Calculator = new Md5Calculator();
            password = Md5Calculator.CalculateMD5Hash(password);
            AccountModel accountModel = new AccountModel();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || accountModel.login(username, password) == null)
            {
                return Json(new { success = false });
            }
            else
            {
                SessionPersister.username = username;
                var userDetails = db.users.Where(a => a.user_name == username && a.password == password).FirstOrDefault();
                Session["userid"] = userDetails.user_id;
                Session["username"] = userDetails.user_name;
                Session["role"] = userDetails.role;
                Session["is_logged_in"] = true;
                int userid = Convert.ToInt32(userDetails.user_id);
                if (userDetails.role.ToString() == "employer")
                {
                    int emp_id = (from p in db.employer_info where p.login_info == userid select  p.emp_id).FirstOrDefault();
                    string profileimage = (from p in db.employer_info where p.login_info == userid select p.logo).FirstOrDefault();
                    string name = (from p in db.employer_info where p.login_info == userid select p.company_name).FirstOrDefault();
                    employer_info em = db.employer_info.Find(emp_id);
                    em.updated_at = DateTime.Now;
                    db.SaveChanges();
                    Session["name"] = name;
                    Session["emp_id"] = emp_id;
                    Session["profileimage"] = profileimage;
                    return Json(new { success = true, username = username, role = "employer" });

                }
                else
                {
                    string sessid = Guid.NewGuid().ToString();
                    Session["sessid"] = sessid;
                    int js_id = (from p in db.jobseeker_info where p.login_info == userid select p.js_id).FirstOrDefault();
                    jobseeker_info js = db.jobseeker_info.Find(js_id);
                    js.updated_at = DateTime.Now;
                    db.SaveChanges();
                    string profileimage = js.profile ;
                    string name = js.full_name;
                    Session["name"] = name;
                    Session["js_id"] = js_id;
                    Session["profileimage"] = profileimage;
                    Session["emailverified"] = js.email_verified;
                    return Json(new { success = true, username = username, role = "jobseeker" });
                }
            }
        }

        [HttpPost]
        public ActionResult logOn(string username = "", string password = "", string returnurl = "")
        {
            Md5Calculator Md5Calculator = new Md5Calculator();
            password = Md5Calculator.CalculateMD5Hash(password);
            AccountModel accountModel = new AccountModel();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || accountModel.login(username, password) == null)
            {
                ViewBag.message = "<div class='alert alert-danger'>Wrong Credentials</div>";
                return View("login");
            }
            else
            {
                SessionPersister.username = username;
                var userDetails = db.users.Where(a => a.user_name == username && a.password == password).FirstOrDefault();
                Session["userid"] = userDetails.user_id;
                Session["username"] = userDetails.user_name;
                Session["role"] = userDetails.role;
                Session["is_logged_in"] = true;
                int userid = Convert.ToInt32(userDetails.user_id);
                if (userDetails.role.ToString() == "employer")
                {
                    int emp_id = (from p in db.employer_info where p.login_info == userid select p.emp_id).FirstOrDefault();
                    employer_info em = db.employer_info.Find(emp_id);
                    em.updated_at = DateTime.Now;
                    db.SaveChanges();
                    string profileimage = (from p in db.employer_info where p.login_info == userid select p.logo).FirstOrDefault();
                    Session["emp_id"] = emp_id;
                    Session["profileimage"] = profileimage;
                    if (!string.IsNullOrEmpty(returnurl))
                    {
                        return Redirect(returnurl);
                    }
                    else
                    {
                        return RedirectToAction("managejobs", "Employer");
                    }
                }
                else
                {
                    int js_id = (from p in db.jobseeker_info where p.login_info == userid select p.js_id).FirstOrDefault();
                    jobseeker_info js = db.jobseeker_info.Find(js_id);
                    js.updated_at = DateTime.Now;
                    db.SaveChanges();
                    string profileimage = (from p in db.jobseeker_info where p.login_info == userid select p.profile).FirstOrDefault();
                    Session["js_id"] = js_id;
                    Session["profileimage"] = profileimage;
                    if (!string.IsNullOrEmpty(returnurl))
                    {
                        return Redirect(returnurl);
                    }
                    else
                    {
                        return RedirectToAction("savedjobs", "jobseeker");
                    }
                }
            }
        }

        public ActionResult Logout()
        {
            SessionPersister.username = string.Empty;
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }
    }
}