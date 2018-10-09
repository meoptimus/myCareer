using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class InterestViewModel
    {
        public int int_id { get; set; }
        public Nullable<int> job_id { get; set; }
        public Nullable<int> jobseeker_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<int> status { get; set; }
    }
}