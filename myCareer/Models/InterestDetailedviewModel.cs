using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class InterestDetailedviewModel
    {
        public int int_id { get; set; }
        public Nullable<int> job_id { get; set; }
        public Nullable<int> jobseeker_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<int> status { get; set; }
        public string  job { get; set; }
        public string jobseeker { get; set; }
        public string employer { get; set; }
        public int emp_id { get; set; }

        public string image { get; set; }
    }
}