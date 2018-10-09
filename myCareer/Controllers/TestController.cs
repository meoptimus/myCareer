using myCareer.Apriori;
using myCareer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myCareer.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        myCareerEntities db = new myCareerEntities();
        public ActionResult Index()
        {
            Apriori.Apriori app = new Apriori.Apriori();
            ViewBag.show = app.str;
            return View();
        }
    }
}