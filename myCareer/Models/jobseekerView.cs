using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myCareer.Models
{
    public class jobseekerView
    {
        [Key]
        public int js_id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only ")]
        public string full_name { get; set; }
        public string current_address { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]

        public string email { get; set; }
        public string email_verified { get; set; }
        [Required]

        public string contact { get; set; }
        public string contact_verified { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string status { get; set; }
        public string facebooklogin_status { get; set; }
        public string facebook_id { get; set; }
        public Nullable<int> login_info { get; set; }
        [Required]

        public string user_name { get; set; }
        [Required]
        public string gender { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string dateofbirth { get; set; }
        [Required]

        public string profile { get; set; }
        public string skills { get; set; }
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string confirm_password { get; set; }
    }
}