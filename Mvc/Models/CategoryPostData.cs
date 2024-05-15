using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitefinity_Web.Mvc.Models
{

        public class CategoryPostData
        {
            public CategoryPostApi[] Property1 { get; set; }
        }

        public class CategoryPostApi
    {
            public int id { get; set; }
            public int count { get; set; }
            public string description { get; set; }
            public string link { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string taxonomy { get; set; }
            public int parent { get; set; }
            public object[] meta { get; set; }
            public object[] acf { get; set; }
            public _Linkss _links { get; set; }
        }

        public class _Linkss
        {
            public Selfs[] self { get; set; }
            public Collections[] collection { get; set; }
            public Abouts[] about { get; set; }
            public WpPost_Type[] wppost_type { get; set; }
            public Curys[] curies { get; set; }
        }

        public class Selfs
        {
            public string href { get; set; }
        }

        public class Collections
        {
            public string href { get; set; }
        }

        public class Abouts
        {
            public string href { get; set; }
        }

        public class WpPost_Type
        {
            public string href { get; set; }
        }

        public class Curys
        {
            public string name { get; set; }
            public string href { get; set; }
            public bool templated { get; set; }
        }

    }