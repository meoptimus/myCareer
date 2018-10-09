using myCareer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;
using myCareer.Security;
using LinqKit;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using myCareer.Apriori;

namespace myCareer.Controllers
{
    public class HomeController : Controller
    {
        myCareerEntities db = new myCareerEntities();

        // GET: Home

        public ActionResult Index()
        {
            ViewBag.category = new SelectList(db.job_categories.ToList(), "jobcat_id", "category_name");
            HomeViewModel hm = new HomeViewModel();
            hm.activeEmployers = db.employer_info.Where(a => a.status == "ACTIVE").Count();
            hm.activeJObseekers = db.jobseeker_info.Where(a => a.status == "ACTIVE").Count();
            hm.activeJobs = db.jobs.Count();
            List<job_model> temp = new List<job_model>();
            var job = db.jobs.Where(a => a.status == 1).OrderByDescending(c => Guid.NewGuid()).Take(6).ToList();
            foreach (var item in job)
            {
                job_model jobmodel = new job_model();
                jobmodel.category = item.category.ToString();
                jobmodel.city = item.city;
                jobmodel.created_at = item.created_at;
                jobmodel.description = item.description;
                jobmodel.district = item.district;
                var name = db.employer_info
                            .Where(c => c.emp_id == item.employer)
                            .Select(cc => cc.company_name)
                            .SingleOrDefault();
                jobmodel.employer = name.ToString();
                jobmodel.full_address = item.full_address;
                jobmodel.state = item.state;
                jobmodel.district = item.district;
                jobmodel.job_id = item.job_id;
                jobmodel.title = item.title;
                jobmodel.job_type = item.job_type;
                jobmodel.emp_id = item.employer.HasValue ? Convert.ToInt32(item.employer) : 0;
                jobmodel.category = db.job_categories.Find(Convert.ToInt32(item.category)).category_name.ToString();
                jobmodel.image =(from m in db.employer_info where m.emp_id==item.employer select m.logo ).Single().ToString();
                jobmodel.submission_date = item.submission_date;
                temp.Add(jobmodel);
            }
            hm.jobs = temp;
            return View(hm);
        }
        public ActionResult EmployerList(int? page,string time="", string cat="",string searchkey="" )
        {
            return View();
        }

        public ActionResult employer_listajax(int? page, string time = "", string cat = "", string searchkey = "", string display="")
        {
            if (display == "" || display == "grid")
                ViewBag.display = "grid";
            else
            {
                                   
                ViewBag.display = "list";
            }
            DateTime today = DateTime.Now;
            int span;
            int pageSize = 15;
            int pageIndex = 1;
            var predicate = PredicateBuilder.New<employer_info>(true);
            if (!string.IsNullOrEmpty(cat))
            {
                string[] cat_ = cat.Split(',');
                foreach (var i in cat_)
                {
                    int j = (Convert.ToInt16(i));
                    predicate = predicate.Or(p => p.company_type == j);
                }
            }
            predicate.And(p => p.company_name.Contains(searchkey));
            predicate = predicate.And(a => a.status == "ACTIVE");
            pageIndex = page.HasValue ? Convert.ToInt16(page) : 1;
            var emp = db.employer_info.Where(predicate).Join(db.company_type, p => p.company_type, pc => pc.id, (p, pc) => new { p = p, category_name = pc.category_name }).ToList();
            //var emp = (from p in db.employer_info
            //join q in db.company_type on p.company_type equals q.id
            //select new { p, q.category_name }
            //).ToList();
            List<employerList> data = new List<employerList>();
            foreach (var item in emp)
            {
                employerList e = new employerList();
                e.category_name = item.category_name;
                e.emp_id = item.p.emp_id;
                e.logo = item.p.logo;
                e.company_name = item.p.company_name;
                e.postedjobs = db.jobs.Where(a => a.employer == e.emp_id && a.status == 1).Count();
                e.Address = item.p.address;
                e.email = item.p.email;
                e.updated_at = DateTime.Parse(item.p.updated_at.ToString());
                e.company_type = item.p.company_type.HasValue ? Convert.ToInt16(item.p.company_type) : 0;
                data.Add(e);
            }
            if (!string.IsNullOrEmpty(time))
            {
                if (time == "all")
                {
                    span = 2500000;
                }
                else
                {
                    span = Convert.ToInt32(time);
                }
                foreach (var variable in data.ToList())
                {
                    TimeSpan t = (today - Convert.ToDateTime(variable.updated_at));
                    double days = t.TotalHours;
                    if (!(days <= span))
                    {
                        data.Remove(variable);
                    }
                }
            }
            return PartialView(data.ToPagedList(pageIndex, pageSize));
        }

        [AllowAnonymous]
        public ActionResult jobListing(int? page, string keyword = "", string city = "", string level = "", string time = "", string cat = "", string type = "")
        {
            return View();

        }
        public ActionResult jobListingajax(int? page, string keyword = "", string city = "", string level = "", string time = "", string cat = "", string type = "")
        {
            
            DateTime today = DateTime.Now;
            int span;

            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt16(page) : 1;
            var predicate1 = PredicateBuilder.New<job>(true);
            var predicate2 = PredicateBuilder.New<job>(true);
            List<job> job = new List<Models.job>();
            if (!string.IsNullOrEmpty(keyword))
                predicate1.And(p => p.title.Contains(keyword));
            if (!string.IsNullOrEmpty(city))
                predicate1.And(p => p.city.Contains(city));
            if (!string.IsNullOrEmpty(level))
                predicate1.And(p => p.job_level.Equals(level));
            predicate1.And(a => a.status == 1);

            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(level) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(time) && string.IsNullOrEmpty(cat) && string.IsNullOrEmpty(type))
            {
                if (Session["js_id"] != null)
                {
                    int ssid = Convert.ToInt16(Session["js_id"].ToString());
                    string skills = db.jobseeker_info.Where(a => a.js_id == ssid).Select(a => a.skills).First();
                    predicate2.And(a => a.status == 1);
                    if (skills != null)
                    {
                        foreach (var d in skills.Split(',').ToList())
                        {
                            job.AddRange(db.jobs.Where(a => a.status == 1 && a.skills.Contains(d)).ToList());
                        }
                    }
                    job.AddRange(db.jobs.Where(a => a.status == 1));
                    job = job.GroupBy(g => g.job_id).OrderByDescending(g => g.Count()).SelectMany(g=>g).ToList();
                    job = job.Distinct().ToList();

                }
                else
                {
                    job = db.jobs.Where(predicate1).OrderByDescending(c => Guid.NewGuid()).ToList();

                }
            }
            else
            {
                job = db.jobs.Where(predicate1).OrderByDescending(c => Guid.NewGuid()).ToList();
            }
           // predicate1.And(a => a.submission_date > DateTime.Now);         
            List<job_model> jobs = new List<job_model>();
            List<job_model> temp = new List<job_model>();
            List<job_model> jobsfinal = new List<job_model>();
            List<job_model> temptype = new List<job_model>();
            foreach (var item in job)
            {
                job_model jobmodel = new job_model();
                jobmodel.category = item.category.ToString();
                jobmodel.city = item.city;
                jobmodel.created_at = item.created_at;
                jobmodel.description = item.description;
                jobmodel.district = item.district;
                jobmodel.job_level = item.job_level;
                jobmodel.gender = item.gender;
                jobmodel.salaryfrom = item.salaryfrom;
                jobmodel.salaryto = item.salaryto;
                jobmodel.experience = item.experience;
                employer_info name = db.employer_info
                            .Where(c => c.emp_id == item.employer)
                            .SingleOrDefault();
                jobmodel.image = name.logo;
                jobmodel.submission_date = item.submission_date;
                jobmodel.employer = name.company_name;
                jobmodel.full_address = item.full_address;
                jobmodel.state = item.state;
                jobmodel.district = item.district;
                jobmodel.job_id = item.job_id;
                jobmodel.title = item.title;
                jobmodel.job_type = item.job_type;
                jobmodel.emp_id = item.employer.HasValue ? Convert.ToInt32(item.employer) : 0;
                jobmodel.category = db.job_categories.Find(Convert.ToInt32(item.category)).category_name.ToString();
                jobs.Add(jobmodel);
            }
            jobsfinal = jobs;
            var predicate = PredicateBuilder.New<job_model>(true);
            if (!string.IsNullOrEmpty(cat))
            {
                string[] cat_ = cat.Split(',');
                foreach (string item in cat_)
                {
                         int item__ = Convert.ToInt16(item);
                         string name = db.job_categories
                            .Where(c => c.jobcat_id == item__)
                            .Select(cc => cc.category_name)
                            .SingleOrDefault().ToString();
                    predicate.Or(p => p.category.Equals(name));              
                }
            }

            if (!string.IsNullOrEmpty(type))
            {
                string[] type_ = type.Split(',');
                foreach (var text in type_)
                {
                        predicate.Or(p=>p.job_type.Contains(text));
                }
            }
            jobsfinal = jobsfinal.Where(predicate).ToList();

            if (!string.IsNullOrEmpty(time))
            {
                if (time == "all")
                {
                    span = 2500000;
                }
                else
                {
                    span = Convert.ToInt32(time);
                }
                foreach (var variable in jobsfinal.ToList())
                {
                    TimeSpan t = (today - Convert.ToDateTime(variable.created_at));
                    double days = t.TotalHours;
                    if (!(days <= span))
                    {
                        jobsfinal.Remove(variable);
                    }
                }
            }

            if (!string.IsNullOrEmpty(type) || !string.IsNullOrEmpty(cat) || !string.IsNullOrEmpty(time))
            {
                return PartialView(jobsfinal.ToPagedList(pageIndex, pageSize));

            }
            else
            {
                return PartialView(jobsfinal.ToPagedList(pageIndex, pageSize));
            }



        }

        public JsonResult jsonCategory()
        {
            var jsonData = db.job_categories
                  .OrderBy(c => Guid.NewGuid()).Take(6)
                  .ToList();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jsonCategoryAll()
        {
            var jsonData = db.job_categories
                  .ToList();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jsonCompany()
        {
            var jsonData = db.company_type
                  .OrderBy(c => Guid.NewGuid()).Take(6)
                  .ToList();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jsonCompanyAll()
        {
            var jsonData = db.company_type
                  .ToList();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult jobdetail(int? id)
        {
            if (Session["js_id"] != null)
            {
                string sessid = Session["sessid"].ToString();
                itemset itemset = db.itemsets.Find(sessid);
                Recommendation rm = new Recommendation();
                List<job> recommended = new List<job>();
                if (itemset != null)
                {

                    string temp = itemset.items;
                    if (string.IsNullOrEmpty(temp))
                    {
                        temp += id.ToString();
                    }
                    else
                    {
                        int status = 0;
                        var strarr = temp.Split(',').ToList();
                        foreach (var ichek in strarr)
                        {
                            int icheck_ = Convert.ToInt32(ichek);
                            if (icheck_ == id)
                            {
                                status = 1;
                                break;
                            }
                        }
                        if (status == 0)
                        {
                            temp += "," + id.ToString();
                            itemset.items = temp;
                        }
                    }
                    db.SaveChanges();
                }
                else
                {
                    itemset itemsetnew = new itemset();
                    itemsetnew.session_id = Session["sessid"].ToString();
                    itemsetnew.items = id.ToString();
                    db.itemsets.Add(itemsetnew);
                    db.SaveChanges();
                }
                List<string> jobids = rm.recommendedjobs(Session["sessid"].ToString());
                if (jobids != null)
                {
                    foreach (var tempIds in jobids)
                    {
                        int intjob = Convert.ToInt16(tempIds);
                        var next = db.jobs.Where(a => a.job_id == intjob && a.status == 1).FirstOrDefault();
                        if (next != null)
                            recommended.Add(next);
                    }
                    recommended = recommended.Distinct().ToList();
                  
                    List<job_model> jobsfinal = new List<job_model>();
                    foreach (var item in recommended)
                    {
                        job_model jobmodel = new job_model();
                        jobmodel.category = item.category.ToString();
                        employer_info name = db.employer_info
                                    .Where(c => c.emp_id == item.employer)
                                    .SingleOrDefault();
                        jobmodel.image = name.logo;
                        jobmodel.submission_date = item.submission_date;
                        jobmodel.employer = name.company_name;
                        jobmodel.full_address = item.full_address;
                        jobmodel.job_id = item.job_id;
                        jobmodel.title = item.title;
                        jobmodel.job_type = item.job_type;
                        jobmodel.emp_id = item.employer.HasValue ? Convert.ToInt32(item.employer) : 0;
                        jobmodel.category = db.job_categories.Find(Convert.ToInt32(item.category)).category_name.ToString();
                        jobsfinal.Add(jobmodel);
                    }
                    ViewBag.recommended = jobsfinal;
                    ViewBag.recognize = jobids;
                }
            }
            if (id != null)
            { 
              
                
                jobdetails job_det = new jobdetails();
                var data  = (from p in db.jobs
                             join q in db.employer_info on p.employer equals q.emp_id
                             join r in db.job_categories on p.category equals r.jobcat_id
                             where p.job_id==id select new {p , q.company_name,q.logo,q.emp_id,r.category_name }).FirstOrDefault();
                List<job> jobs = new List<Models.job>();
                if (data != null)
                {
                    job_det.emp_id = data.emp_id;
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
                    job_det.skills = data.p.skills;
                    job_det.submission_date = data.p.submission_date;
                    job_det.title = data.p.title;
                    jobs = db.jobs.ToList();
                }
                ViewBag.logo = data.logo;
                int appcount = 0;
                jobs = jobs.Where(c=>c.employer==data.p.employer && c.job_id!=id).Take(5).ToList();
                if (Session["role"] != null && Session["role"].ToString() == "jobseeker") {
                    int userid = Convert.ToInt32(Session["userid"].ToString());
                    userid = (from r in db.jobseeker_info where r.login_info == userid select r.js_id).FirstOrDefault();
                    int count = (from p in db.applications
                                       where p.job_id == id
                                       where p.jobseeker_id ==userid
                                       where p.status==1
                                       select p.job_id
                                        ).Count();
                    ViewBag.count = count;
                    int shortlist = (from exa in db.interests where exa.job_id == id where exa.jobseeker_id == userid where exa.status == 1 select exa.int_id).Count();
                    ViewBag.interested = shortlist;
                }
                appcount = (from f in db.interests where f.status == 1 where f.job_id == id select f.int_id).Count();
                ViewBag.appcount = appcount;
                ViewBag.data = jobs.ToList();

                return View(job_det);
            }
            else
            {
                return RedirectToAction("index","login");
            }
        }
        public ActionResult employer_profile(int? emp_id)
        {
            if (emp_id.HasValue)
            {
                int emp_id_ = Convert.ToInt32(emp_id);
                var b = (from p in db.employer_info
                           join q in db.company_type on p.company_type equals q.id
                            where p.emp_id==emp_id_
                           select new { p, q.category_name }).FirstOrDefault();
                employerList e = new employerList();
                e.Address = b.p.address;
                e.category_name = b.category_name;
                e.company_name = b.p.company_name;
                e.created_at = DateTime.Parse(b.p.created_at.ToString());
                e.description = b.p.description;
                e.email = b.p.email;
                e.contact = b.p.contact;
                e.logo = b.p.logo;
                e.emp_id = b.p.emp_id;

                List<job> job = new List<Models.job>();
                job = db.jobs.Where(a => a.status == 1 && a.employer == emp_id_).Take(5).ToList();
                ViewBag.postedjobs = db.jobs.Where(a => a.status == 1 && a.employer == emp_id_).Count();
                List<job_model> jobs = new List<job_model>();
                foreach (var item in job)
                {
                    job_model jobmodel = new job_model();
                    jobmodel.category = item.category.ToString();
                    jobmodel.city = item.city;
                    jobmodel.created_at = item.created_at;
                    jobmodel.description = item.description;
                    jobmodel.district = item.district;
                    var name = db.employer_info
                                .Where(c => c.emp_id == item.employer)
                                .Select(cc => cc.company_name)
                                .SingleOrDefault();
                    jobmodel.employer = name.ToString();
                    jobmodel.full_address = item.full_address;
                    jobmodel.state = item.state;
                    jobmodel.district = item.district;
                    jobmodel.job_id = item.job_id;
                    jobmodel.title = item.title;
                    jobmodel.job_type = item.job_type;
                    var image = db.employer_info
                                .Where(c => c.emp_id == item.employer)
                                .Select(cc => cc.logo)
                                .SingleOrDefault();
                    jobmodel.image = image;
                    jobmodel.category = db.job_categories.Find(Convert.ToInt32(item.category)).category_name.ToString();
                    jobs.Add(jobmodel);
                }
                ViewBag.jobs = jobs;
                return View(e);

            }
            else {
                return Json("Restricted Access",JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> sendMessge(string name, string email, string phone, string message, int emp_id)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(message))
            {
                employer_info emp = db.employer_info.Find(emp_id);
                message mess = new Models.message();
                mess.message1 = message;
                mess.created_at = DateTime.Now;
                mess.read_status = 0;
                mess.sender_address = email;
                mess.sender_contact = phone;
                mess.status = 1;
                db.messages.Add(mess);
                if (db.SaveChanges() > 0)
                {
                    await SendEmail(name, emp.email, phone, message, emp.company_name);
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

        public ActionResult verification(string token)
        {
             email_verification email = db.email_verification.Where(a=>a.token.Equals(token) && a.status==1).FirstOrDefault();
            if (email != null)
            {
                DateTime now = DateTime.Now;
                TimeSpan t = (now - Convert.ToDateTime(email.created_at));
                double min = t.TotalMinutes;
                if (min > 60)
                {
                    return Json("Token Expired", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    email.status = 0;
                    int js = email.js_id.HasValue ? Convert.ToInt32(email.js_id) : 0;
                    jobseeker_info jsl = db.jobseeker_info.Find(js);
                    if (jsl != null)
                    {
                        jsl.email_verified = "TRUE";
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json("Invalid Token", JsonRequestBehavior.AllowGet);

                    }

                    db.SaveChanges();
                    return RedirectToAction("index", "Login");
                }
            }
            else
            {
                return Json("Invalid Token", JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]

        public ActionResult jobseeker_profile(int id=0)
        {
            jobseeker_info js = db.jobseeker_info.Find(id);
            if (js != null)
            {
                JobseekerDetailedModel a = new JobseekerDetailedModel();
                a.contact = js.contact;
                a.created_at = js.created_at;
                a.current_address = js.current_address;
                a.dateofbirth = js.dateofbirth;
                a.education = db.jobseeker_education.Where(p => p.js_id == id).ToList();
                a.email = js.email;
                a.full_name = js.full_name;
                a.gender = js.gender;
                a.js_id = js.js_id;
                a.profile = js.profile;
                a.skills = js.skills;
                a.updated_at = js.updated_at;
                a.facebook_id = js.facebook_id;
                a.experience = db.js_experience.Where(l =>l.js_id == id).ToList();
                return View(a);
                
            }
            else
            {
                return Json("Restricted Access", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> jobseekermessage(string name, string email, string phone, string message, int js_id)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(message))
            {
                jobseeker_info js = db.jobseeker_info.Find(js_id);
                message mess = new Models.message();
                mess.message1 = message;
                mess.created_at = DateTime.Now;
                mess.read_status = 0;
                mess.sender_address = email;
                mess.sender_contact = phone;
                mess.status = 1;
                db.messages.Add(mess);
                if (db.SaveChanges() > 0)
                {
                    await SendEmail(name,js.email,phone,message,js.full_name);
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
        private async System.Threading.Tasks.Task SendEmail(string name, string email, string phone, string messages, string targetname)
        {
            var body = "<p>Dear: {0} </p><p>{1}</p><p> Contact: {2}</p><br/><p> Regards, {3}</p><p>MyCareer online job portal</p>";
            var message = new System.Net.Mail.MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
            message.From = new MailAddress("noreply@mycareer.com");  // replace with valid value
            message.Subject = "Message";
            message.Body = string.Format(body,targetname, messages, phone, name);
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
        public ActionResult cvdownload(int js_id)
        {
            jobseeker_info js = db.jobseeker_info.Find(js_id);
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("CvPdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            string strAttachment = Server.MapPath("~/Content/pdfs/" + strPDFFileName);
            status.Append("<!DOCTYPE html>");
            status.Append("<html>");
            status.Append("<head><title></title></head>");
            status.Append("<body style='margin: 20px 100px;'><h2 style='text-align:center'>Curriculum Vitae</h2>");
            status.Append("<b>"+js.full_name+"</b><br>");
            status.Append("<b>(+977)"+js.contact+"</b><br>");
            status.Append("<b>" + js.email + "</b><br>");
            status.Append("<b>" + js.current_address + "</b><br><br>");
            List<jobseeker_education> ed = db.jobseeker_education.Where(a => a.js_id == js_id).OrderByDescending(a=>a.jsed_id).ToList();
            if (ed.Count > 0)
            {
                status.Append("<h3><b>Education</b></h3>");
                foreach (var item in ed)
                {
                    status.Append("<b>"+item.education_level+"</b><br><ul>");
                    status.Append("<li>"+item.start_date+" - "+item.end_date+"</li>");
                    status.Append("<li>"+item.institution+"</li></ul>");
                }
            }
            List<js_experience> ex = db.js_experience.Where(a=>a.js_id==js_id).ToList();
            if (ex.Count > 0)
            {
                status.Append("<h3><b>Experience</b></h3><br>");
                foreach (var item in ex)
                {
                    status.Append("<b>" + item.title + "</b><br><ul>");
                    status.Append("<li>" + item.start_date + " - " + item.end_date + "</li>");
                    status.Append("<li>" + item.institution + "</li></ul>");
                }
            }
           List <string> skills = js.skills.Split(',').ToList();
            if (skills.Count > 0)
            {
                status.Append("<h3><b>Skills</b></h3><br><ul>");
                foreach (var item in ex)
                {
                    status.Append("<b>" + item + "</b><br><br>");
                }
                status.Append("</ul>");

            }
            string cv = db.tbl_cv.Where(a => a.jobseeker_id == js.js_id).Select(a => a.cv).FirstOrDefault().ToString();
            status.Append(cv);
            status.Append("</body>");
            status.Append("</html>");
            StringReader str = new StringReader(status.ToString());
            MemoryStream ms = new MemoryStream();
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4);

            PdfWriter pdfwriter = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
            document.Open();
            using (TextReader sReader = new StringReader(status.ToString()))
            {
                List<IElement> list = HTMLWorker.ParseToList(sReader, new StyleSheet());
                foreach (IElement elm in list)
                {
                    document.Add(elm);
                }
            }
            document.Close();
            document.Dispose();
            Response.Buffer = true;
            Response.Clear();
            Response.AppendHeader("Content-Type", "application/x-pdf");
            Response.AppendHeader("Content-disposition", "attachment; filename="+js.full_name+"cv.pdf");
            Response.BinaryWrite(ms.GetBuffer());
            Response.Flush();
            Response.Close();
            return View();

        }

        public ActionResult contactus()
        {
            return View();
        }
        public ActionResult PageNotFound()
        {
            return View();
        }
    }
}