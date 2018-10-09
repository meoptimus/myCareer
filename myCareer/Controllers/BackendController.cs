using myCareer.Models;
using myCareer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myCareer.Controllers
{
    public class BackendController : Controller
    {
        myCareerEntities db = new myCareerEntities();
            [AllowAnonymous]
        public ActionResult Index()
        {
            return View("Login");
        }
        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            Md5Calculator md5 = new Md5Calculator();
            password = md5.CalculateMD5Hash(password);
            var userDetails = db.users.Where(a => a.user_name == username && a.password == password && a.role=="admin").FirstOrDefault();
            if (userDetails != null)
            {
                return RedirectToAction("employersList", "Backend");
            }
            else
            {
                ViewBag.error = true;
                ViewBag.errorMsg = "Either username or password is invalid";
                return View("Login");
            }
        }
        public ActionResult employersList()
        {
            var employerList = db.employer_info.ToList();
            ViewBag.list = employerList;
            return View("employersList");
        }

        public ActionResult jobseekersList()
        {
            var employerList = db.jobseeker_info.ToList();
            ViewBag.list = employerList;
            return View("jobseekerList");
        }

        public ActionResult Industries()
        {
            var company_type = db.company_type.ToList();
            foreach (var item in company_type) ;
            ViewBag.list = company_type;
            return View("Industries");
        }
        [HttpPost]


        public ActionResult addIndustry(string industryName)
        {
            if (String.IsNullOrEmpty(industryName.Trim()))
            {
                return Json(new { errorMsg = "Industry name is required" });
            }
            var tempInd = db.company_type.Where(a => a.category_name == industryName).FirstOrDefault();
            if (tempInd == null)
            {
                company_type company = new company_type();
                company.category_name = industryName.Trim();
                company.status = "TRUE";
                db.company_type.Add(company);
                if (db.SaveChanges() > 0)
                {
                    return Json(new { successMsg = "Sucessfully Added!" ,insertId=company.id});

                }
                else
                {
                    return Json(new { errorMsg = "Something went wrong!" });
                    
                }
            }
            else
            {
                return Json(new { errorMsg = "Category already exists!" });

            }

        }
        [HttpPost]

        public ActionResult changeIndustryStatus(string id)
        {
            int id_ = Convert.ToInt16(id);
            string status;
            var tempInd = db.company_type.Where(a => a.id == id_).FirstOrDefault();
            if (tempInd.status.ToString() == "TRUE")
            {
                status = "FALSE";
            }
            else
            {
                status = "TRUE";
            }
            var toUpdate = db.company_type.SingleOrDefault(x => x.id == id_);

            if (toUpdate != null)
                toUpdate.status = status;
            if (db.SaveChanges() > 0)
            {
                return Json(new { success = "true", status = status });
            }
            else
            {
                return Json(new { errorMsg="Cannot Update" });

            }

        }

        public ActionResult changeEmployerStatus(string id)
        {
            int id_ = Convert.ToInt16(id);
            string status;
            var tempInd = db.employer_info.Where(a => a.emp_id == id_).FirstOrDefault();
            if (tempInd.status.ToString() == "ACTIVE")
            {
                status = "INACTIVE";
            }
            else
            {
                status = "ACTIVE";
            }
            var toUpdate = db.employer_info.SingleOrDefault(x => x.emp_id == id_);

            if (toUpdate != null)
                toUpdate.status = status;
            if (db.SaveChanges() > 0)
            {
                return Json(new { success = "true", status = status });
            }
            else
            {
                return Json(new { errorMsg = "Cannot Update" });

            }
        }

        public ActionResult changeJobseekerStatus(string id)
        {
            int id_ = Convert.ToInt16(id);
            string status;
            var tempInd = db.jobseeker_info.Where(a => a.js_id == id_).FirstOrDefault();
            if (tempInd.status.ToString() == "ACTIVE")
            {
                status = "INACTIVE";
            }
            else
            {
                status = "ACTIVE";
            }
            var toUpdate = db.jobseeker_info.SingleOrDefault(x => x.js_id == id_);

            if (toUpdate != null)
                toUpdate.status = status;
            if (db.SaveChanges() > 0)
            {
                return Json(new { success = "true", status = status });
            }
            else
            {
                return Json(new { errorMsg = "Cannot Update" });

            }

        }
        public ActionResult jobs()
        {
            List<job> job = db.jobs.ToList();
            List<job_model> jobs = new List<job_model>();
            foreach (var item in job)
            {
                job_model jobmodel = new job_model();
                jobmodel.category = item.category.ToString();
                jobmodel.city = item.city;
                jobmodel.created_at = item.created_at;
                jobmodel.description = item.description;
                jobmodel.district = item.district;
                employer_info name = db.employer_info
                            .Where(c => c.emp_id == item.employer)
                            .SingleOrDefault();
                jobmodel.image = name.logo;
                jobmodel.salaryfrom = item.salaryfrom;
                jobmodel.salaryto = item.salaryto;
                jobmodel.job_level = item.job_level;
                jobmodel.status = Convert.ToInt32(item.status);
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
            return View(jobs);
        }
        public ActionResult jobDetail(int job_id)
        {
            return PartialView(db.jobs.Where(a => a.job_id == job_id).FirstOrDefault());
        }
        public ActionResult jobseekerdetail(int js_id)
        {
            return PartialView(db.jobseeker_info.Where(a=>a.js_id==js_id).First());
        }
        public ActionResult disableJob(int job_id)
        {
            job job = db.jobs.Find(job_id);
            job.status = (job.status + 1) % 2;
            db.SaveChanges();
             return RedirectToAction("jobs","backend");
        }
        public ActionResult disableApp(int app_id)
        {
            application job = db.applications.Find(app_id);
            job.status = (job.status + 1) % 2;
            db.SaveChanges();
            return RedirectToAction("applications", "backend");
        }
        public ActionResult Applications()
        {
            var data = (from p in db.applications
                        join q in db.jobs on p.job_id equals q.job_id
                        join r in db.employer_info on q.employer equals r.emp_id
                        join s in db.jobseeker_info on p.jobseeker_id equals s.js_id
                        select new
                        {
                            p,
                            q.title,
                            r.company_name,
                            r.logo,
                            r.emp_id,
                            s.full_name
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
                interest.job = item.title;
                interest.jobseeker = item.full_name;
                interest.employer = item.company_name;
                interest.employer_id = item.emp_id;
                interest.accept_status = item.p.accept_status;
                int_list.Add(interest);

            }
        
            return View(int_list);
        }
    }
}