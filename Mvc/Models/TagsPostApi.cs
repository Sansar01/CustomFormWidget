using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitefinity_Web.Mvc.Models
{
    public class TagsPostApi
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Taxonomy { get; set; }
        public List<Link> Links { get; set; }
    }

    public class Link
    {
        public string Self { get; set; }
        public string Collection { get; set; }
        public string About { get; set; }
        public string PostType { get; set; }
        public List<TagCury> Curies { get; set; }
    }

    public class TagCury
    {
        public string Name { get; set; }
        public string Href { get; set; }
        public bool Templated { get; set; }
    }
}