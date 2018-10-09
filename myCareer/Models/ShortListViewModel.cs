using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class ShortListViewModel
    {
        public int app_id { get; set; }
        public string job { get; set; }
        public string jobseeker { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> job_id { get; set; }
        public Nullable<int> jobseeker_id { get; set; }
        public string profile { get; set; }
        public int age { get; set; }
        public string address { get; set; }
    }
}