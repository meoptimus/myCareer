using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class HomeViewModel
    {
        public int activeJobs { get; set; }
        public int activeEmployers { get; set; }
        public int activeJObseekers { get; set; }
        public List<job_model> jobs { get; set; }
    }
}