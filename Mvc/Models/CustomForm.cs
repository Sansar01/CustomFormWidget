using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Descriptors;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.ModuleEditor.Web.Templates;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Services.RelatedData.Responses;
using Telerik.Web.UI;
using Telerik.Web.UI.ExportInfrastructure;

namespace Sitefinity_Web.Mvc.Models
{
    public class CustomForm
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }    


        public string Category { get; set; }


        public Image ItemImage { get;set; }

        public static implicit operator CustomForm(string v)
        {
            throw new NotImplementedException();
        }
    }
}