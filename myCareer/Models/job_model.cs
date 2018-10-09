using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myCareer.Models
{
    public class job_model
    {
        [Key]
        [Required]
        public int job_id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        [DataType(DataType.Date)]

        public Nullable<System.DateTime> submission_date { get; set; }
        [Required]

        public string category { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string description { get; set; }
        [Required]

        public string job_type { get; set; }
        [Required]

        public string job_level { get; set; }
        [Required]

        public string gender { get; set; }

        public Nullable<double> salaryfrom { get; set; }

        public Nullable<double> salaryto { get; set; }
        public Nullable<int> experience { get; set; }
        public string state { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string postalcode { get; set; }
        [Required]
        public string full_address { get; set; }
        public int emp_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        [Required]
        public Nullable<int> openings { get; set; }
        public string employer { get; set; }
        [Required]
        public string skills { get; set; }
        public string image { get; set; }
        public int status { get; set; }
    }
}