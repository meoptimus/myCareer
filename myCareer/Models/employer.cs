using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myCareer.Models
{
    public class employer
    {
        [Key]
        public int emp_id { get; set; }

        [StringLength(100)]
        [Required]
        public string company_name { get; set; }

        [Required]

        public int? company_type { get; set; }

        [StringLength(20)]
        [Required]

        public string phone { get; set; }

        [StringLength(100)]
        [Required]

        public string address { get; set; }

        [Column(TypeName = "text")]
        [Required]

        public string description { get; set; }

        public int? reg_type { get; set; }

        [StringLength(50)]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only ")]
        public string name { get; set; }

        [StringLength(50)]
        [Required]
        [DataType(DataType.EmailAddress)]


        public string email { get; set; }

        [StringLength(50)]
        [Required]

        public string contact { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        [StringLength(50)]
        public string status { get; set; }

        //[Required(ErrorMessage = "Please select file.")]
        public HttpPostedFileBase logo { get; set; }

        public Nullable<int> login_info { get; set; }
        [Required]
        [Remote("doesUsernameExists", "Employer", ErrorMessage = "Username already exists")]

        public string user_name { get; set; }
        [Required]

        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string confirm_password { get; set; }


    }
}