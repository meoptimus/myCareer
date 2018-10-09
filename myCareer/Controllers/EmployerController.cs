using myCareer.Models;
using myCareer.Security;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace myCareer.Controllers
{
    public class EmployerController : Controller
    {
        // GET: Employer
        myCareerEntities db = new myCareerEntities();

        public JsonResult doesUsernameExists(string user_name)
        {
            var validateName = db.users.FirstOrDefault
                                (x => x.user_name == user_name);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
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


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            myCareerEntities db = new myCareerEntities();
            ViewBag.category = new SelectList(db.company_type.ToList(), "id", "category_name");
            ViewBag.registration_type = new SelectList(db.registration_type.ToList(), "id", "registration_name");
            return View();
        }
        [HttpPost]
        public ActionResult Register(HttpPostedFileBase logo, employer model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    user tempuser = new user();
                    employer_info employer = new employer_info();
                    tempuser.user_name = model.user_name;
                    tempuser.password = CalculateMD5Hash(model.password);
                    tempuser.role = "employer";
                    tempuser.status = "ACTIVE";
                    db.users.Add(tempuser);
                    if (db.SaveChanges() > 0)
                    {
                        string filename = null;

                        if (logo != null)
                        {
                            string pic = Path.GetExtension(logo.FileName);
                            filename = Guid.NewGuid().ToString() + pic;
                            string path = Path.Combine(Server.MapPath("~/Content/images"), filename);
                            logo.SaveAs(path);
                        }

                        int insertId = tempuser.user_id;
                        employer.company_type = model.company_type;
                        employer.company_name = model.company_name;
                        employer.address = model.address;
                        employer.contact = model.contact;
                        employer.description = model.description;
                        employer.email = model.email;
                        employer.name = model.name;
                        employer.phone = model.phone;
                        employer.reg_type = model.reg_type;
                        employer.login_info = insertId;
                        employer.status = "ACTIVE";
                        employer.created_at = DateTime.Now;
                        employer.updated_at = DateTime.Now;
                        employer.logo = filename;
                        db.employer_info.Add(employer);
                        db.SaveChanges();
                        Session["name"] = employer.company_name ;
                        Session["userid"] = employer.login_info;
                        Session["role"] = "employer";
                        Session["profileimage"] = employer.logo;

                        Session["emp_id"] = employer.emp_id;
                        Session["is_logged_in"] = true;
                        Session["username"] = model.user_name;

                        return RedirectToAction("EmployerProfile", "Employer");
                    }
                    else
                    {
                        return Json(new { errorMsg = "Cannot Insert!" });

                    }


                }
                catch (Exception ex)
                {
                    return Json(new { errorMsg = "Something went wrong! Error message (" + ex.ToString() + ")" });

                }
            }
            else
            {
                return Json(new { errorMsg = "validation error!" });
            }
        }
        [HttpGet]
        [MyAuthorizeAttribute(Roles = "employer")]
        public ActionResult EmployerProfile()
        {
            ViewBag.activeStatus = "profile";
            ViewBag.company = new SelectList(db.company_type.ToList(), "id", "category_name");
            ViewBag.registration_type = new SelectList(db.registration_type.ToList(), "id", "registration_name");
            int id = Convert.ToInt32(Session["userid"]);
            var data = db.employer_info.Where(a => a.login_info == id).FirstOrDefault();
            var social = db.employer_social.Where(a => a.emp_id == data.emp_id).FirstOrDefault();
            if (social != null)
            {
                ViewBag.facebook = social.facebook;
                ViewBag.google = social.google;
                ViewBag.youtube = social.youtube;
                ViewBag.website = social.website;
                ViewBag.twitter = social.twitter;
            }
            return View(data);
        }
        [ValidateInput(false)]
        [HttpPost]
        [MyAuthorizeAttribute(Roles = "employer")]
        public ActionResult EmployerProfile(string emp_id,string company_name,string email,string phone, string reg_type,string company_type,string address, string description, string facebook="",string youtube="",string google="",string website="", string twitter="" )
        {
            int emp_id_ = Convert.ToInt16(emp_id);
            employer_info e = db.employer_info.Find(emp_id_);
            e.address = address;
            e.company_name = company_name;
            e.company_type = Convert.ToInt16(company_type);
            e.phone = phone;
            e.description = description;
            e.reg_type = Convert.ToInt16(reg_type);
            e.updated_at = DateTime.Now;
            e.email = email;
            if (db.SaveChanges() > 0)
            {
                ViewBag.edit = "<div class='alert alert-success'>Sucessfully updated</div>";

                employer_social soc = db.employer_social.Where(a => a.emp_id == emp_id_).FirstOrDefault();
                if (soc != null)
                {
                    soc.emp_id = Convert.ToInt16(emp_id);
                    soc.facebook = facebook;
                    soc.youtube = youtube;
                    soc.twitter = twitter;
                    soc.website = website;
                    soc.google = google;
                    db.SaveChanges();
                }
                else
                {
                    employer_social soc1 = new employer_social();
                    soc1.facebook = facebook;
                    soc1.youtube = youtube;
                    soc1.twitter = twitter;
                    soc1.website = website;
                    soc1.google = google;
                    db.employer_social.Add(soc1);
                    db.SaveChanges();
                }
 
            }
            else
            {
                ViewBag.edit = "<div class='alert alert-alert'>Cannnot Update</div>";
            }
            return RedirectToAction("EmployerProfile","employer");
        }
        [HttpGet]
        [MyAuthorize(Roles = "employer")]

        public ActionResult postJob()
        {
            ViewBag.category = new SelectList(db.company_type.ToList(), "id", "category_name");
            ViewBag.districts = new SelectList(db.nepal_district.ToList(), "district_name", "district_name");

            return View();
        }
        [HttpPost]
        [MyAuthorize(Roles = "employer")]

        public ActionResult postJob(job_model model)
        {
            ViewBag.category = new SelectList(db.company_type.ToList(), "id", "category_name");
            ViewBag.districts = new SelectList(db.nepal_district.ToList(), "district_name", "district_name");
            if (ModelState.IsValid)
            {
                try
                {
                    int sessionid = Convert.ToInt32(Session["userid"].ToString());
                    job job = new job();
                    job.title = model.title;
                    job.submission_date = Convert.ToDateTime(model.submission_date);
                    job.description = model.description;
                    job.category = Convert.ToInt16(model.category);
                    job.job_type = model.job_type;
                    job.job_level = model.job_level;
                    job.gender = model.gender;
                    job.salaryfrom = model.salaryfrom;
                    job.salaryto = model.salaryto;
                    job.experience = Convert.ToInt32(model.experience);
                    job.state = model.state;
                    job.district = model.district;
                    job.city = model.city;
                    job.postalcode = model.postalcode;
                    job.full_address = model.full_address;
                    job.created_at = DateTime.Now;
                    job.openings = model.openings;
                    job.status = 1;
                    job.skills = model.skills;
                    job.employer = (from a in db.employer_info where a.login_info == sessionid select a.emp_id).FirstOrDefault();
                    db.jobs.Add(job);
                    if (db.SaveChanges() > 0)
                    {
                        ViewBag.message = "<div class='alert alert-success'><center>Sucessfully Inserted!</center></div>";
                        ModelState.Clear();
                    }
                    else
                    {
                        ViewBag.message = "<div class='alert alert-danger'>Something went wrong!</div>";

                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return View("postJob");
        }

        [HttpGet]
        [MyAuthorize(Roles = "employer")]

        public ActionResult manageJobs(string searchkey, int? page)
        {
            int sess = Convert.ToInt32(Session["emp_id"].ToString());
            searchkey = string.IsNullOrWhiteSpace(searchkey) ? "" : searchkey;
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt16(page) : 1;
            List<jobdetails> list = new List<jobdetails>();
            List <job> jobs = db.jobs.Where(c => c.status == 1 &&  c.employer == sess && (c.title.Contains(searchkey) || c.description.Contains(searchkey) )).ToList();
            foreach (var item in jobs)
            {
                jobdetails job = new jobdetails();
                job.applications = db.applications.Where(a => a.job_id == item.job_id && a.status == 1).Count();
                int jobcatid = Convert.ToInt16(item.category);
                job.category_name = db.job_categories.Where(a => a.jobcat_id == jobcatid).Select(a => a.category_name).FirstOrDefault();
                job.city = item.city;
                job.created_at = item.created_at;
                job.description = item.description;
                job.submission_date = item.submission_date;
                job.title = item.title;
                job.job_id = item.job_id;
                list.Add(job);

            }
            return View(list.ToPagedList(pageIndex, pageSize));
        }
        [MyAuthorizeAttribute(Roles = "employer")]

        public ActionResult jobDetails(string id = null)
        {

            if (id == null)
            {
                return RedirectToAction("manageJobs", "Employer");
            }
            else
            {
                int id_;
                if (int.TryParse(id, out id_))
                {
                    id_ = Convert.ToInt32(id);
                    var data  = (from p in db.jobs
                                join q in db.employer_info on p.employer equals q.emp_id
                                join r in db.job_categories on p.category equals r.jobcat_id
                                where p.job_id == id_
                                select new { p, q.company_name, r.category_name }).FirstOrDefault();

                        jobdetails job_det = new jobdetails();
                        job_det.category_name = data.category_name;
                        job_det.city = data.p.city;
                        job_det.created_at = data.p.created_at;
                        job_det.description = data.p.description;
                        job_det.district = data.p.district;
                        job_det.employer = data.company_name;
                        job_det.experience = data.p.experience;
                        job_det.full_address = data.p.full_address;
                        job_det.gender = data.p.gender;
                        job_det.job_id = data.p.job_id;
                        job_det.job_level = data.p.job_level;
                        job_det.job_type = data.p.job_type;
                        job_det.openings = data.p.openings;
                        job_det.postalcode = data.p.postalcode;
                        job_det.salaryfrom = data.p.salaryfrom;
                        job_det.salaryto = data.p.salaryto;
                        job_det.state = data.p.state;
                        job_det.status = data.p.status;
                        job_det.submission_date = data.p.submission_date;
                        job_det.title = data.p.title;
                        job_det.skills = data.p.skills;

                    return View(job_det);
                }
                else
                {
                    ViewBag.errorMsg = "Invalid Job";
                    return View();
                }

            }
        }
        [HttpGet]
        [MyAuthorize(Roles = "employer")]
        public ActionResult editDetails(int id)
        {
            ViewBag.category = new SelectList(db.company_type.ToList(), "id", "category_name");
            ViewBag.districts = new SelectList(db.nepal_district.ToList(), "district_name", "district_name");
            int id_;
            id_ = Convert.ToInt32(id);
            job job_model = db.jobs.Find(id_);
            return View(job_model);
        }
        [HttpPost]
        [MyAuthorize(Roles = "employer")]
        public ActionResult editDetails(job_model model)
        {
            ViewBag.category = new SelectList(db.job_categories.ToList(), "jobcat_id", "category_name");
            ViewBag.districts = new SelectList(db.nepal_district.ToList(), "district_name", "district_name");
            if (ModelState.IsValid)
            {
                try
                {
                    job job = new job();
                    job = db.jobs.Find(model.job_id);
                    job.title = model.title;
                    job.submission_date = Convert.ToDateTime(model.submission_date);
                    job.description = model.description;
                    job.category = Convert.ToInt16(model.category);
                    job.job_type = model.job_type;
                    job.job_level = model.job_level;
                    job.gender = model.gender;
                    job.salaryfrom = model.salaryfrom;
                    job.salaryto = model.salaryto;
                    job.experience = Convert.ToInt32(model.experience);
                    job.state = model.state;
                    job.district = model.district;
                    job.city = model.city;
                    job.postalcode = model.postalcode;
                    job.full_address = model.full_address;
                    job.created_at = DateTime.Now;
                    job.openings = model.openings;
                    job.status = 1;
                    job.skills = model.skills;
                    if (db.SaveChanges() > 0)
                    {
                        ViewBag.message = "<div class='alert alert-success'><center>Sucessfully Inserted!</center></div>";
                        ModelState.Clear();
                        return RedirectToAction("jobDetails", new RouteValueDictionary(
                                                            new { controller = "employer", action = "jobDetails", id = job.job_id }));
                    }
                    else
                    {
                        ViewBag.message = "<div class='alert alert-danger'>Something went wrong!</div>";
                        return View();
                    }
                    
                }
                catch (Exception ex)
                {
                    ViewBag.error = ex.ToString();
                    return View();
                }

            }
            else {
                return View();

            }
        }

        [MyAuthorize(Roles = "employer")]
        public ActionResult deleteJob(string id)
        {
            int id_;
            if (int.TryParse(id, out id_))
            {
                id_ = Convert.ToInt32(id);
                job job = new job();
                var data = db.jobs.Find(id_);
                if (data != null)
                {
                    job = db.jobs.Find(id_);
                    job.status = 0;
                    db.Entry(job).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("managejobs","employer");
            }
            else
            {
                ViewBag.errorMsg = "Invalid Job";
                return RedirectToAction("managejobs", "employer");

            }

        }
        [MyAuthorize(Roles ="employer")]
        public ActionResult interestedCandidates(int? page,int? job_id, string searchkey="")
        {
            int pageIndex;
            int emp_id = Convert.ToInt32(Session["emp_id"].ToString());
            pageIndex = page.HasValue ? Convert.ToInt16(page) : 1;
            int pageSize = 10;
            int userid = Convert.ToInt32(Session["emp_id"].ToString());
            List<ApplicationViewModel> show = new List<ApplicationViewModel>();
            if (job_id.HasValue)
            {
                job_id = Convert.ToInt16(job_id);
                var model = (from data in db.applications
                             join js in db.jobseeker_info on data.jobseeker_id equals js.js_id
                             join jobs in db.jobs on data.job_id equals jobs.job_id
                             join emp in db.employer_info on jobs.employer equals emp.emp_id
                             where emp.emp_id == userid
                             where jobs.status == 1
                             where data.status == 1
                             where data.job_id==job_id
                             where js.full_name.Contains(searchkey) || jobs.title.Contains(searchkey)
                             where jobs.employer == emp.emp_id
                             select new
                             {
                                 app_id = data.app_id,
                                 job = jobs.title,
                                 jobseeker = js.full_name,
                                 created_at = data.created_at,
                                 status = data.status,
                                 job_id = jobs.job_id,
                                 jobseeker_id = js.js_id,
                                 accept_staus = data.accept_status
                             }
                   );
                foreach (var item in model)
                {
                    ApplicationViewModel app = new ApplicationViewModel();
                    app.app_id = item.app_id;
                    app.job = item.job;
                    app.jobseeker = item.jobseeker;
                    app.jobseeker_id = item.jobseeker_id;
                    app.status = item.status;
                    app.job_id = item.job_id;
                    app.created_at = item.created_at;
                    app.accept_status = item.accept_staus;
                    show.Add(app);
                }
            }
            else {
                var model = (from data in db.applications
                             join js in db.jobseeker_info on data.jobseeker_id equals js.js_id
                             join jobs in db.jobs on data.job_id equals jobs.job_id
                             join emp in db.employer_info on jobs.employer equals emp.emp_id
                             where emp.emp_id == userid
                             where jobs.status == 1
                             where data.status == 1
                             where js.full_name.Contains(searchkey) || jobs.title.Contains(searchkey)
                             where jobs.employer == emp.emp_id
                             select new
                             {
                                 app_id = data.app_id,
                                 job = jobs.title,
                                 jobseeker = js.full_name,
                                 created_at = data.created_at,
                                 status = data.status,
                                 job_id = jobs.job_id,
                                 jobseeker_id = js.js_id,
                                 accept_staus = data.accept_status
                             }
                   );
                foreach (var item in model)
                {
                    ApplicationViewModel app = new ApplicationViewModel();
                    app.app_id = item.app_id;
                    app.job = item.job;
                    app.jobseeker = item.jobseeker;
                    app.jobseeker_id = item.jobseeker_id;
                    app.status = item.status;
                    app.job_id = item.job_id;
                    app.created_at = item.created_at;
                    app.accept_status = item.accept_staus;
                    show.Add(app);
                }

            }
          
            return View(show.ToPagedList(pageIndex,pageSize));
        }
        [MyAuthorize(Roles = "employer")]

        public async System.Threading.Tasks.Task<ActionResult> shortlist(int? app_id)
        {
            if (app_id.HasValue)
            {
                int app_id_ = Convert.ToInt32(app_id);
                application app = new application();
                app = db.applications.Find(app_id_);
                if (app != null)
                {
                    app.accept_status = 1;
                    job job = db.jobs.Find(app.job_id);
                    string title = job.title;
                    int emp_id = Convert.ToInt16(job.employer);
                    string sender = db.employer_info.Find(emp_id).company_name;
                    string email = db.jobseeker_info.Find(app.jobseeker_id).email;
                    string targetname = db.jobseeker_info.Find(app.jobseeker_id).full_name;
                    string message = "You have been sucessfully selected for the job named " + title + " . please contact " + sender + "as soon as possible. <br/> Auto generated email at " + DateTime.Now; ;
                   await SendEmail(sender, email, message, targetname);
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
                    return Json("Restricted Access", JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                return Json("Restricted Access", JsonRequestBehavior.AllowGet);
            }

        }
        private async System.Threading.Tasks.Task SendEmail(string name, string email, string messages, string targetname)
        {
            var body = "<p>Dear: {0} </p><p>{1}</p><br/><p> Regards, {2}</p><p>MyCareer online job portal</p>";
            var message = new System.Net.Mail.MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
            message.From = new MailAddress("noreply@mycareer.com");  // replace with valid value
            message.Subject = "Shortlisted for the job ";
            message.Body = string.Format(body, targetname, messages, name);
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
                    ViewBag.error = "<div class='alert alert-danger'>" + ex.ToString() + "</div>";

                }

            }
        }

        [MyAuthorize(Roles = "employer")]

        public ActionResult removeShortlist(int? app_id)
        {
            if (app_id.HasValue)
            {
                int app_id_ = Convert.ToInt32(app_id);
                    application app = db.applications.Find(app_id_);
                if (app != null)
                {
                    app.accept_status = 0;
                    if (db.SaveChanges() > 0)
                    {
                        return RedirectToAction("interestedCandidates");

                    }
                    else
                    {
                        return RedirectToAction("interestedCandidates");
                    }

                }
                return RedirectToAction("interestedCandidates");
            }
            else
            {
                return HttpNotFound();
            }
        }
        [MyAuthorize(Roles = "employer")]
        public ActionResult shortListedCandidates(int? page, int? job_id, string searchkey="")
        {
            int pageIndex;
            int emp_id = Convert.ToInt32(Session["emp_id"].ToString());
            pageIndex = page.HasValue ? Convert.ToInt16(page) : 1;
            int pageSize = 10;
            int userid = Convert.ToInt32(Session["emp_id"].ToString());
            List<ShortListViewModel> show = new List<ShortListViewModel>();
            if (job_id.HasValue)
            {
                int job_id_ = Convert.ToInt16(job_id);
                var model = (from data in db.applications
                             join js in db.jobseeker_info on data.jobseeker_id equals js.js_id
                             join jobs in db.jobs on data.job_id equals jobs.job_id
                             where jobs.employer == emp_id
                             where jobs.status == 1
                             where jobs.job_id==job_id_
                             where data.accept_status == 1
                             where data.status == 1
                             where js.full_name.Contains(searchkey) || jobs.title.Contains(searchkey)

                             select new
                             {
                                 app_id = data.app_id,
                                 job = jobs.title,
                                 jobseeker = js.full_name,
                                 created_at = data.created_at,
                                 status = data.status,
                                 job_id = jobs.job_id,
                                 jobseeker_id = js.js_id,
                                 profile = js.profile,
                                 age = js.dateofbirth,
                                 location = js.current_address
                             }
                  );
                foreach (var item in model)
                {
                    ShortListViewModel app = new ShortListViewModel();
                    app.app_id = item.app_id;
                    app.job = item.job;
                    app.jobseeker = item.jobseeker;
                    app.jobseeker_id = item.jobseeker_id;
                    app.status = item.status;
                    app.job_id = item.job_id;
                    app.created_at = item.created_at;
                    app.profile = item.profile;
                    var temp = item.age;
                    if (temp != null)
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan t = (now - Convert.ToDateTime(temp));
                        int years = Convert.ToInt16(t.TotalDays) / 365;
                        app.age = years;
                    }
                    show.Add(app);
                }
            }
            else
            {
                var model_job_id = (from data in db.applications
                             join js in db.jobseeker_info on data.jobseeker_id equals js.js_id
                             join jobs in db.jobs on data.job_id equals jobs.job_id
                             where jobs.employer == emp_id
                             where jobs.status == 1
                             where data.accept_status == 1
                             where data.status == 1
                             where js.full_name.Contains(searchkey) || jobs.title.Contains(searchkey)

                             select new
                             {
                                 app_id = data.app_id,
                                 job = jobs.title,
                                 jobseeker = js.full_name,
                                 created_at = data.created_at,
                                 status = data.status,
                                 job_id = jobs.job_id,
                                 jobseeker_id = js.js_id,
                                 profile = js.profile,
                                 age = js.dateofbirth,
                                 location = js.current_address
                             }
                  );
                foreach (var item in model_job_id)
                {
                    ShortListViewModel app = new ShortListViewModel();
                    app.app_id = item.app_id;
                    app.job = item.job;
                    app.jobseeker = item.jobseeker;
                    app.jobseeker_id = item.jobseeker_id;
                    app.status = item.status;
                    app.job_id = item.job_id;
                    app.created_at = item.created_at;
                    app.profile = item.profile;
                    var temp = item.age;
                    if (temp != null)
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan t = (now - Convert.ToDateTime(temp));
                        int years = Convert.ToInt16(t.TotalDays) / 365;
                        app.age = years;
                    }
                    show.Add(app);
                }

            }

            return View(show.ToPagedList(pageIndex, pageSize));
        }
        [MyAuthorize(Roles = "employer")]
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
               employer_info emp = db.employer_info.Find(Convert.ToInt16(Session["emp_id"].ToString()));
                emp.logo = filename;
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
        [HttpGet]
        [MyAuthorize(Roles = "employer")]
        public ActionResult changePassword()
        {
            return View();
        }
        [HttpPost]
        [MyAuthorize(Roles = "employer")]
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
                    else {
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
        [HttpPost]
        [MyAuthorize(Roles = "employer")]

        public ActionResult removeAcceptStatus(int app_id)
        {
            application app = db.applications.Find(app_id);
            if (app != null)
            {
                app.accept_status = 0;
                if (db.SaveChanges() > 0)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);

            }
        }

    }
}
