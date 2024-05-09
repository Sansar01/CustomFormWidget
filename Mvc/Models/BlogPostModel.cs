using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitefinity_Web.Mvc.Models
{
    public class BlogPostModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }


        public string Category { get; set; }

        public static explicit operator string(BlogPostModel v)
        {
            throw new NotImplementedException();
        }
    }
}