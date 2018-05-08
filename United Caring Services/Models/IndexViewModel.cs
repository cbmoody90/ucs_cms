using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace United_Caring_Services.Models
{
    public class HomeViewModel
    {
        public string vision { get; set; }
        public List<string> needs { get; set; } = new List<string>();
     
    }
}