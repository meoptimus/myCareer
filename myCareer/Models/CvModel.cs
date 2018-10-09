using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myCareer.Models
{
    public class CvModel
    {
        public int cv_id { get; set; }
        public Nullable<int> jobseeeker_id { get; set; }
        public string file_loc { get; set; }
        [AllowHtml]
        public string cv { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}