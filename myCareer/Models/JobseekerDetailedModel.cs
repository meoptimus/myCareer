using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class JobseekerDetailedModel
    {
        public int js_id { get; set; }
        public string full_name { get; set; }
        public string current_address { get; set; }
        public string email { get; set; }
        public string email_verified { get; set; }
        public string contact { get; set; }
        public string contact_verified { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string status { get; set; }
        public string facebooklogin_status { get; set; }
        public string facebook_id { get; set; }
        public Nullable<int> login_info { get; set; }
        public string profile { get; set; }
        public string skills { get; set; }
        public string gender { get; set; }
        public Nullable<System.DateTime> dateofbirth { get; set; }
        public List<jobseeker_education> education { get; set; }
        public List<js_experience> experience { get; set; }

        public List<ApplicationDetailedViewModel> jobsApplied { get; set; }

    }
}