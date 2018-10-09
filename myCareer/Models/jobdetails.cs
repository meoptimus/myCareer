using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace myCareer.Models
{
    public class jobdetails
    {
        [Key]
        public int job_id { get; set; }
        public string title { get; set; }
        public Nullable<System.DateTime> submission_date { get; set; }
        public string category { get; set; }
        public string category_name { get; set; }
        public string description { get; set; }
        public string job_type { get; set; }
        public string job_level { get; set; }
        public string gender { get; set; }
        public Nullable<double> salaryfrom { get; set; }
        public Nullable<double> salaryto { get; set; }
        public Nullable<int> experience { get; set; }
        public string state { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string postalcode { get; set; }
        public string full_address { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<int> openings { get; set; }
        public Nullable<int> status { get; set; }
        public string employer { get; set; }
        public int emp_id { get; set; }
        public int applications { get; set; }
        public string skills { get; set; }
    }
}