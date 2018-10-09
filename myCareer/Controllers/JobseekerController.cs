using myCareer.Models;
using myCareer.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Globalization;

namespace myCareer.Controllers
{
    public class JobseekerController : Controller
    {
        // GET: Jobseeker
        myCareerEntities db = new myCareerEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(HttpPostedFileBase profile,jobseekerView model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    user tempuser = new user();
                    jobseeker_info jobseeker = new jobseeker_info();
                    tempuser.user_name = model.user_name;
                    tempuser.password = CalculateMD5Hash(model.password);
                    tempuser.role = "jobseeker";
                    tempuser.status = "ACTIVE";
                    db.users.Add(tempuser);
                    if (db.SaveChanges() > 0)
                    {
                        string filename = null;
                        if (profile != null)
                        {
                            string pic = Path.GetExtension(profile.FileName);
                            filename = Guid.NewGuid().ToString() + pic;
                            string path = Path.Combine(Server.MapPath("~/Content/images"), filename);
                            profile.SaveAs(path);
                        }
                        int insertId = tempuser.user_id;
                        jobseeker.full_name = model.full_name;
                        jobseeker.current_address = model.current_address;
                        jobseeker.email = model.email;
                        jobseeker.contact = model.contact;
                        jobseeker.created_at = DateTime.Now;
                        jobseeker.updated_at = DateTime.Now;
                        jobseeker.status = "ACTIVE";
                        jobseeker.facebooklogin_status = "FALSE";
                        jobseeker.email_verified = "FALSE";
                        jobseeker.login_info = insertId;
                        jobseeker.profile = filename;
                        jobseeker.gender = model.gender;
                        jobseeker.dateofbirth = Convert.ToDateTime(model.dateofbirth);
                        db.jobseeker_info.Add(jobseeker);
                        db.SaveChanges();
                        string sessid = Guid.NewGuid().ToString();
                        Session["sessid"] = sessid;
                        Session["profileimage"] = jobseeker.profile;
                        Session["name"] = jobseeker.full_name;
                        Session["userid"] = jobseeker.login_info;
                        Session["username"] = tempuser.user_name;
                        Session["role"] = tempuser.role;
                        Session["is_logged_in"] = true;
                        Session["js_id"] = jobseeker.js_id;
                        Session["emailverified"] = jobseeker.email_verified;

                        return RedirectToAction("jobseekerDashboard", "Jobseeker");
                    }
                    else
                    {
                        ViewBag.message = "Error";
                        return View();
                    }


                }
                catch (Exception ex)
                {
                    ViewBag.message = "exceptio "+ex.ToString();
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
         [MyAuthorize(Roles = "jobseeker")]
        public JsonResult applyForTheJob( int? job_id)
        {
            if (!(Session["userid"] == null && job_id == null))
            {
                int sessionid = Convert.ToInt32(Session["userid"].ToString());
                int jobseeker_id = (from p in db.jobseeker_info
                                    where p.login_info == sessionid
                                    select p.js_id).FirstOrDefault();
                application app = new application();
                app.jobseeker_id =jobseeker_id;
                app.created_at = DateTime.Now;
                app.job_id = job_id;
                app.status = 1;
                app.accept_status = 0;
                int checkExists = (from q in db.applications where q.jobseeker_id == jobseeker_id where q.job_id == job_id select q.app_id).Count();
                if (checkExists == 0)
                {
                    db.applications.Add(app);
                    if (db.SaveChanges() > 0)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);

                    }
                }
                else {
                    return Json(false, JsonRequestBehavior.AllowGet);

                }

            }
            else {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [MyAuthorize(Roles = "jobseeker")]
        public ActionResult interestAdd(int? job_id)
        {
            if (!(Session["userid"] == null && job_id == null))
            {
                int jobseeker_id = Convert.ToInt32(Session["js_id"].ToString());
                interest app = new interest();
                app.jobseeker_id = jobseeker_id;
                app.created_at = DateTime.Now;
                app.job_id = job_id;
                app.status = 1;
                int checkExists = (from q in db.interests where q.jobseeker_id == jobseeker_id where q.job_id == job_id where q.status==1 select q.int_id).Count();
                if (checkExists == 0)
                {
                    db.interests.Add(app);
                    if (db.SaveChanges() > 0)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [MyAuthorize(Roles = "jobseeker")]

        public ActionResult jobseekerDashboard()
        {
            JobseekerDetailedModel data = new JobseekerDetailedModel();
            int sess_id = Convert.ToInt32(Session["js_id"].ToString());
            var temp = (from p in db.jobseeker_info
                        where p.js_id == sess_id 
                        
                        select p).FirstOrDefault();
            data.contact = temp.contact;
            data.created_at = temp.created_at;
            data.current_address = temp.current_address;
            data.dateofbirth = temp.dateofbirth;
            data.education = db.jobseeker_education.Where(a => a.js_id == sess_id).ToList<jobseeker_education>();
            data.dateofbirth = temp.dateofbirth;
            data.email = temp.email;
            data.email_verified = temp.email_verified;
            data.full_name = temp.full_name;
            data.gender = temp.gender;
            data.skills = temp.skills;
            data.updated_at = temp.updated_at;
            data.profile = temp.profile;
            data.created_at = temp.created_at;
            return View(data);
        }
        [MyAuthorize(Roles = "jobseeker")]

        public ActionResult savedJobs(int? page, string searchkey="")
        {
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 10;
            int js_id = Convert.ToInt32(Session["js_id"].ToString());
            var data = (from p in db.interests
                        join q in db.jobs on p.job_id equals q.job_id
                        join r in db.employer_info on q.employer equals r.emp_id
                        join s in db.jobseeker_info on p.jobseeker_id equals s.js_id
                        where p.jobseeker_id == js_id
                        where r.name.Contains(searchkey) || q.title.Contains(searchkey)
                        where p.status == 1
                        select new
                        {
                            p,
                            q.title,
                            r.company_name,
                            r.logo,
                            r.emp_id
                        }).ToList();

            List<InterestDetailedviewModel> int_list = new List<InterestDetailedviewModel>();
            foreach (var item in data)
            {
                InterestDetailedviewModel interest = new InterestDetailedviewModel();
                interest.created_at = item.p.created_at;
                interest.int_id = item.p.int_id;
                interest.jobseeker_id = item.p.jobseeker_id;
                interest.job_id = item.p.job_id;
                interest.status = item.p.status;
                interest.image = item.logo;
                interest.employer = item.company_name;
                interest.emp_id = item.emp_id;
                interest.job = item.title;
                int_list.Add(interest);
            }
            return View(int_list.ToPagedList(pageIndex, pageSize));
        }
        [MyAuthorize(Roles = "jobseeker")]

        public ActionResult removeInterest(int int_id)
        {
            interest sh = db.interests.Find(int_id);
            if (sh != null)
            {
                sh.status = 0;
                if (db.SaveChanges() > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            return Json(false, JsonRequestBehavior.AllowGet);

        }

        [MyAuthorize(Roles = "jobseeker")]
        public ActionResult appliedJobs(int? page, string searchkey="")
        {
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            int pageSize = 10;
            int js_id = Convert.ToInt32(Session["js_id"].ToString());
            var data = (from p in db.applications
                        join q in db.jobs on p.job_id equals q.job_id
                        join r in db.employer_info on q.employer equals r.emp_id
                        join s in db.jobseeker_info on p.jobseeker_id equals s.js_id
                        where p.jobseeker_id == js_id
                        where r.name.Contains(searchkey) || q.title.Contains(searchkey)
                        where p.status == 1
                        select new
                        {
                            p,
                            q.title,
                            r.company_name,
                            r.logo,
                            r.emp_id
                        }).ToList();
            List<ApplicationDetailedViewModel> int_list = new List<ApplicationDetailedViewModel>();
            foreach (var item in data)
            {
                ApplicationDetailedViewModel interest = new ApplicationDetailedViewModel();
                interest.created_at = item.p.created_at;
                interest.app_id = item.p.app_id;
                interest.jobseeker_id = item.p.jobseeker_id;
                interest.job_id = item.p.job_id;
                interest.status = item.p.status;
                interest.job = item.company_name;
                interest.job = item.title;
                interest.employer = item.company_name;
                interest.employer_id = item.emp_id;
                interest.accept_status = item.p.accept_status;
                int_list.Add(interest);

            }
            return View(int_list.ToPagedList(pageIndex, pageSize));
        }
        [MyAuthorize(Roles = "jobseeker")]

        public RedirectToRouteResult removeApplication(int app_id)
        {
            string sess = Session["js_id"].ToString();
            int sess_ = Convert.ToInt32(sess);
            application app = db.applications.Where(a=>a.app_id==app_id && a.jobseeker_id==sess_).FirstOrDefault();
            if (app != null)
            {
                app.status = 0;
                db.SaveChanges();
                return RedirectToAction("appliedjobs", "jobseeker");
            }
            else {
                return RedirectToAction("appliedjobs", "jobseeker");
            }
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpGet]
        public ActionResult ManageResume()
        {
            return View();
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public ActionResult ManageResume(HttpPostedFileBase cv)
        {
            int sess = Convert.ToInt32(Session["js_id"].ToString());
            string filename = null;
            if (cv != null)
            {
                try
                {
                    string ext = Path.GetExtension(cv.FileName);
                    filename = Guid.NewGuid().ToString() + ext;
                    string path1 = Path.Combine(Server.MapPath("~/Content/Files"), filename);
                    cv.SaveAs(path1);
                    tbl_cv cv_save =db.tbl_cv.Where(a=>a.jobseeker_id==sess).FirstOrDefault();
                    if (cv_save == null)
                    {
                        tbl_cv cvnext = new tbl_cv();
                        cvnext.jobseeker_id = sess;
                        cvnext.file_loc = filename;
                        cvnext.updated_at = DateTime.Now;
                        db.tbl_cv.Add(cvnext);
                    }
                    else
                    {
                        cv_save.file_loc = filename;
                        cv_save.updated_at = DateTime.Now;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        StringBuilder text = new StringBuilder();
                        Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                        object miss = System.Reflection.Missing.Value;
                        object path = @"E:\College_project\myCareer_files\myCareer\myCareer\Content\Files\" + filename;
                        object readOnly = true;
                        Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
                        docs.Activate();
                        for (int i = 0; i < docs.Paragraphs.Count; i++)
                        {
                            text.Append(" \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString());
                        }
                        string final = Regex.Replace(text.ToString(), @"\r\n?|\n", "<br/>"); 
                        cv_save.cv = final;
                        db.SaveChanges();
                        return View();

                    }
                    else
                    {
                        return View();
                    }

                   
                }
                catch (Exception ex)
                {
                     ex.StackTrace.ToString();
                }
               
            }
            
            return View();
        }
        [MyAuthorize(Roles = "jobseeker")]

        public JsonResult getCv()
        {
            int sess= Convert.ToInt32(Session["js_id"].ToString());
            var data = db.tbl_cv.Where(a => a.jobseeker_id == sess).Select(a => a.cv).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult updateCv(string val)
        {
            int sess = Convert.ToInt32(Session["js_id"].ToString());
            tbl_cv cv_save = db.tbl_cv.Where(a => a.jobseeker_id == sess).FirstOrDefault();
            cv_save.updated_at = DateTime.Now;
            cv_save.cv = val;
            if (db.SaveChanges() > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
        }
    
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public JsonResult setEducation()
        {
            int sess = Convert.ToInt32(Session["js_id"].ToString());
            var data = db.jobseeker_education.Where(a => a.js_id == sess && a.status == 1);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public JsonResult setExperience()
        {
            int sess = Convert.ToInt32(Session["js_id"].ToString());
            var data = db.js_experience.Where(a => a.js_id == sess && a.status == 1);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]

        public JsonResult removeEducation(int jsed_id)
        {
            jobseeker_education cv = db.jobseeker_education.Find(jsed_id);
            if (cv != null)
            {
                cv.status = 0;
                if (db.SaveChanges() > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]

        public JsonResult removeExperience(int jsed_id)
        {
            js_experience cv = db.js_experience.Find(jsed_id);
            if (cv != null)
            {
                cv.status = 0;
                if (db.SaveChanges() > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public JsonResult addEducation(string title, string start_date, string end_date, string institution)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date) && !string.IsNullOrEmpty(institution))
            {
                jobseeker_education jsed = new jobseeker_education();
                int sess = Convert.ToInt32(Session["js_id"].ToString());
                jsed.education_level = title;
                jsed.status = 1;
                jsed.institution = institution;
                jsed.start_date = DateTime.Parse(start_date);
                jsed.end_date = DateTime.Parse(end_date);
                jsed.js_id = sess;
                db.jobseeker_education.Add(jsed);
                if (db.SaveChanges() > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public JsonResult addExperience(string title, string start_date, string end_date, string institution)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date) && !string.IsNullOrEmpty(institution))
            {
                js_experience jsed = new js_experience();
                int sess = Convert.ToInt32(Session["js_id"].ToString());
                jsed.title = title;
                jsed.status = 1;
                jsed.institution = institution;
                jsed.start_date = DateTime.Parse(start_date);
                jsed.end_date = DateTime.Parse(end_date);
                jsed.js_id = sess;
                db.js_experience.Add(jsed);
                if (db.SaveChanges() > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public ActionResult getSkills()
        {
            List<string> termsList = new List<string>();
            int sess = Convert.ToInt32(Session["js_id"].ToString());
            var skills = (from p in db.jobseeker_info where p.js_id == sess select p.skills).First();

            return Json(skills, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Skills()
        {
            List<string> termsList = new List<string>();
            var skills = (from p in db.jobseeker_info select p.skills).ToList();
          
            foreach (var data in skills) {
                if (data != null)
                {
                    List<string> temp = data.Split(',').ToList();
                    foreach (var v in temp)
                    {
                        termsList.Add(v);
                    }

                }
            }
            return Json(termsList.Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }

        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public JsonResult changeSkills(string val)
        {
            int sess = Convert.ToInt32(Session["js_id"].ToString());
            jobseeker_info js = db.jobseeker_info.Where(a => a.js_id == sess).FirstOrDefault();
            js.skills = val;
            if (db.SaveChanges() > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public void notification()
        {
            Session["notification"] = "FALSE";
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public async Task<JsonResult> verifyEmail()
        {
            int js_id = Convert.ToInt32(Session["js_id"].ToString());
            jobseeker_info js = db.jobseeker_info.Find(js_id);
            if (js.email_verified == "FALSE")
            {
                Random r = new Random();
                int num = r.Next(1000, 9999);
                string temp = CalculateMD5Hash(num.ToString());
                string link = "http://localhost:52920/home/verification/?token=" + temp;
                await SendEmail(js.email, link, js.full_name, js.js_id);
                email_verification email = new email_verification();
                email.js_id = js_id;
                email.created_at = DateTime.Now;
                email.status = 1;
                email.token = temp;
                db.email_verification.Add(email);
                if (db.SaveChanges() > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);

                }
            }
            else {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
        }

        private async System.Threading.Tasks.Task SendEmail(string email, string link, string name, int id)
        {
            var body = "<p>Dear: {0} </p><p>Please Verify Your Email Address: {1}</p><p> goto this link: {2}</p><br/><p> before it expires. Regards,</p><p>MyCareer online job portal</p>";
            var message = new System.Net.Mail.MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
            message.From = new MailAddress("noreply@mycareer.com");  // replace with valid value
            message.Subject = "Verify Your Email address";
            message.Body = string.Format(body, name, email, link);
            message.IsBodyHtml = true;
            int res = 0;
            using (var smtp = new SmtpClient())
            {
                var credential = new System.Net.NetworkCredential
                {
                    UserName = "shpud@tilottama.edu.np",  // replace with valid value
                    Password = "2e@oN;g.tvAA"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "mail.tilottama.edu.np";
                smtp.Port = 587;
                smtp.EnableSsl = false;
                try
                {

                    await smtp.SendMailAsync(message);
                    res = 1;
                }
                catch (Exception ex)
                {
                    ViewBag.error = "<div class='alert alert-danger'>"+ex.ToString()+"</div>";

                }

            }
        }
        [HttpGet]
        [MyAuthorize(Roles = "jobseeker")]
        public ActionResult changePassword()
        {
            return View();
        }
        [HttpPost]
        [MyAuthorize(Roles = "jobseeker")]
        public ActionResult changePassword(string password, string confirmpassword, string oldpassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmpassword) || string.IsNullOrEmpty(oldpassword))
            {
                ViewBag.message = "<div class='alert alert-danger'>Fill up the required fields</div>";
                return View();
            }
            else
            {
                int sess_id = Convert.ToInt16(Session["userid"].ToString());
                user user = db.users.Find(sess_id);
                Md5Calculator md = new Md5Calculator();
                if (md.CalculateMD5Hash(oldpassword) == user.password)
                {
                    if (password == confirmpassword)
                    {
                        user.password = md.CalculateMD5Hash(password);
                        if (db.SaveChanges() > 0)
                        {
                            ViewBag.message = "<div class='alert alert-success'>Sucessfully Updated</div>";
                            return View();
                        }
                        else
                        {
                            ViewBag.message = "<div class='alert alert-danger'>Something went wrong!!</div>";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.message = "<div class='alert alert-danger'>password and confirm password mismatched!!</div>";
                        return View();
                    }
                }
                else
                {
                    ViewBag.message = "<div class='alert alert-danger'>Incorrect old password.</div>";
                    return View();
                }

            }
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public JsonResult changeProfile(HttpPostedFileBase photo)
        {
            string filename = null;
            if (photo != null)
            {
                string pic = Path.GetExtension(photo.FileName);
                filename = Guid.NewGuid().ToString() + pic;
                string path = Path.Combine(Server.MapPath("~/Content/images"), filename);
                photo.SaveAs(path);
                jobseeker_info emp = db.jobseeker_info.Find(Convert.ToInt16(Session["js_id"].ToString()));
                emp.profile = filename;
                if (db.SaveChanges() > 0)
                {
                    Session["profileimage"] = filename;
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
        }
        [MyAuthorize(Roles = "jobseeker")]
        public ActionResult editDetails()
        {
            int sess_id = Convert.ToInt32(Session["js_id"].ToString());
            return PartialView(db.jobseeker_info.Find(sess_id));
        }
        [MyAuthorize(Roles = "jobseeker")]
        [HttpPost]
        public ActionResult editDetails(jobseeker_info model)
        {
            if (ModelState.IsValid)
            {
                jobseeker_info final = db.jobseeker_info.Find(model.js_id);
                final.full_name = model.full_name;
                final.current_address=model.current_address;
                final.contact=model.contact;
                final.skills=model.skills;
                final.gender = model.gender;
                final.dateofbirth = Convert.ToDateTime(model.dateofbirth);
                db.SaveChanges();
                return RedirectToAction("jobseekerdashboard", "jobseeker");
            }
            else
            {
                return RedirectToAction("jobseekerdashboard", "jobseeker");
            }

        }
    }
}