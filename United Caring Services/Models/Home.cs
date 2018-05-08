using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace United_Caring_Services.Models
{
    public class ContentModel
    {
        public string ContentText { get; set; }
    }

    public class flags
    {
        [Display(Name = "White Flag Status")]
        public Boolean Whiteflag { get; set; }
        [Display(Name = "Red Flag Status")]
        public Boolean Redflag { get; set; }
    }

    public class upload
    {
        [Required]
        [Display(Name = "File Name")]
        public string fileName { get; set; }
        public string folder { get; set; }
        [Display(Name = "File")]
        [DataType(DataType.Upload)]
        [Required]
        public HttpPostedFileBase collection { get; set; }
    }

    public class postModel
    {
            public string text{get;set;}
            public string images{get;set;}
            public string urls{get;set;}
            public string urlnames{get;set;}
            public string files{get;set;}
            public string filenames { get; set; }
    }

    public class UserRoles
    {
        [Display(Name = "Email")]
        public string UserName { get; set; }
        [Display(Name = "Role")]
        public string UserRole { get; set; }
    }

    public class Users
    {
        [Display(Name = "Email")]
        public string UserName { get; set; }
    }
}