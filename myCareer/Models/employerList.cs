using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class employerList
    {
        public int emp_id { get; set; }
        public String category_name { get; set; }
        public String phone { get; set; }
        public String Address { get; set; }
        public String description { get; set; }
        public String company_name { get; set; }
        public String registration_name { get; set; }
        public int postedjobs { get; set; }
        public String email { get; set; }
        public String contact { get; set; }
        public String name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int company_type { get; set; }
        public String logo { get; set; }
    }
}