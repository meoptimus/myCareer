using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class Account
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string status { get; set; }
    }
}