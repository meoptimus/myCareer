using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class ApplicationViewModel
    {
        public int app_id { get; set; }
        public string job { get; set; }
        public string jobseeker { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> job_id { get; set; }
        public Nullable<int> jobseeker_id { get; set; }
        public Nullable<int> shortliststatus { get; set; }
        public Nullable<int> accept_status { get; set; }

    }
}