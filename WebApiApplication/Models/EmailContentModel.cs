using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace WebApiApplication.Models
{
    public class EmailContentModel
    {
        [Required]
        [AllowHtml]
        public string EmailContent { get; set; }

        [AllowHtml]
        public string XmlLikeContent { get; set; }
    }
}