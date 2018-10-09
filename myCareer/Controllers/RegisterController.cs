using myCareer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace myCareer.Controllers
{
    public class RegisterController : Controller
    {
        myCareerEntities db = new myCareerEntities();
        // GET: Register
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult jobSeeker()
        {
            return View();

        }

    }
}