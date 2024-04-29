using Sitefinity_Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Security;
using System.Drawing;
using DocumentFormat.OpenXml.Vml;
using System.IO;
using System.Text.RegularExpressions;

using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Modules.Libraries.Web.UI.Designers;
using Image = Telerik.Sitefinity.Libraries.Model.Image;
using Telerik.Sitefinity.Workflow;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Taxonomies.Web;
using Telerik.Web.UI.PageLayout;
using Microsoft.ProjectServer.Client;
using Amazon.S3.Model;
using DocumentFormat.OpenXml.Bibliography;
using static Telerik.Sitefinity.Security.SecurityConstants.Sets;
using Album = Telerik.Sitefinity.Libraries.Model.Album;
using Telerik.Sitefinity.Analytics.Server.Infrastructure.Mappers;




namespace Sitefinity_Web.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "CustomFormWidget", Title = "Custom Form Widget", SectionName = "Custom Widgets")]
    public class CustomFormController : Controller
    {

        Guid albumId;
        string albumTitle = string.Empty;

        // GET: CustomForm
        public ActionResult Index()
        {


            return View("Index");
        }

        [HttpPost]
        public ActionResult SubmitForm(CustomForm customData,HttpPostedFileBase itemImage)
        { 
            var CustomForm = new CustomForm();
            CustomForm.Title = customData.Title;
            CustomForm.Description = customData.Description;
            //CustomForm.Tags = customData.Tags;
            //CustomForm.ItemImage = customData.ItemImage;


            string tags = customData.Tags;
            
            string category = customData.Category;

            


            // Create a new item in the dynamic module
            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
            Type customRecordType = TypeResolutionService.ResolveType("Telerik.Sitefinity.DynamicTypes.Model.Dynamic_Module.Dynamic_module");

            DynamicContent customRecord = dynamicModuleManager.CreateDataItem(customRecordType);


            customRecord.SetString("Title", CustomForm.Title);
            customRecord.SetString("Description", CustomForm.Description);


            // adding tagging to the library

            addTags(tags);
            addTaxon(customRecord, tags);


            //adding category to the library

            addCategory(category);

            addCategories(customRecord,category);


            // creating album 

            addAlbum();

            if (itemImage != null && itemImage.ContentLength > 0)
            {


                // Generate a new GUID for the image
                Guid masterImageId = Guid.NewGuid();
                // Obtain the image stream from the uploaded file
                Stream imageStream = itemImage.InputStream;

                // Obtain the image file name
                string imageFileName = System.IO.Path.GetFileName(itemImage.FileName);

                // Obtain the image extension from the file name
                string imageExtension = System.IO.Path.GetExtension(imageFileName);


                string imageTitle = System.IO.Path.GetFileNameWithoutExtension(imageFileName);


                addImage(masterImageId,imageStream, imageFileName, imageExtension, imageTitle,albumId,customRecord);

            }

              // Publish the item
                customRecord.ApprovalWorkflowState = "Published";

            dynamicModuleManager.SaveChanges();

            return View("Index", CustomForm);
        }


        public void addTags(string tags)
        {
            var taxonomyManager = TaxonomyManager.GetManager();

            //Get the Tags taxonomy
            var tagTaxonomy = taxonomyManager.GetTaxonomies<FlatTaxonomy>().SingleOrDefault(s => s.Name == "Tags");

            if (tagTaxonomy == null) return;

            //Create a new FlatTaxon
            var taxon = taxonomyManager.CreateTaxon<FlatTaxon>();

            //Associate the item with the flat taxonomy
            taxon.FlatTaxonomy = tagTaxonomy;

            taxon.Name = Regex.Replace(tags.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");
            taxon.Title = tags;
            taxon.UrlName = Regex.Replace(tags.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");

            //Add it to the list
            tagTaxonomy.Taxa.Add(taxon);

            taxonomyManager.SaveChanges();

        }

        public void addTaxon(DynamicContent customRecord, string tagname)
        {
            var taxonomyManager = TaxonomyManager.GetManager();
            var Tag = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Taxonomy.Name == "Tags");
 
                foreach (var Tags in Tag.Where(w => w.Title.ToLower() == tagname.ToLower()))
            {
                if (Tags != null)
                {
                    customRecord.Organizer.AddTaxa("Tags", Tags.Id);

                }
            }
        }


        public void addCategory(string category)
        {
            var taxonomyManager = TaxonomyManager.GetManager();

            //Get the Categories taxonomy
            var categoryTaxonomy = taxonomyManager.GetTaxonomies<HierarchicalTaxonomy>().SingleOrDefault(s => s.Name == "Categories");

            if (categoryTaxonomy == null) return;

            //Create a new HierarchicalTaxon
            var taxon = taxonomyManager.CreateTaxon<HierarchicalTaxon>();

            //Associate the item with the hierarchical taxonomy
            taxon.Taxonomy = categoryTaxonomy;

            taxon.Name = Regex.Replace(category.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");
            taxon.Title = category;
            taxon.UrlName = Regex.Replace(category.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");


            //HierarchicalTaxon parentCategory = new HierarchicalTaxon();

            //Check if the parent has been set
            //if (parentCategory != null)
            //{
            //    taxon.Parent = parentCategory;
            //}

            //Add it to the list
            categoryTaxonomy.Taxa.Add(taxon);

            taxonomyManager.SaveChanges();
        }

        public void addCategories(DynamicContent customRecord ,string  categoryName)
        {
            TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
            var Category = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Taxonomy.Name == "Categories");

            foreach (var categorys in Category.Where(w => w.Title.ToLower() == categoryName.ToLower()))
            {

                if (categorys != null)
                {
                    customRecord.Organizer.AddTaxa("Category", categorys.Id);
                }
            }
        }


        public void addAlbum()
        {
            //gets an isntance of the LibrariesManager
            var manager = LibrariesManager.GetManager();
            Album albumManager = manager.GetAlbums().Where(a => a.Title == "ImageAlbumTitle1").FirstOrDefault();

            if(albumManager == null)
            {
                //creates an image album(library)
                var imagesAlbum = manager.CreateAlbum();
                imagesAlbum.Title = "ImageAlbumTitle3";
                manager.SaveChanges();
            }
            else
            {
                checkAlbum(albumManager);
            }

          
        }

        public void checkAlbum(Album imagesAlbum) 
        {
            albumId = imagesAlbum.Id;
            albumTitle = imagesAlbum.Title;
        }

        public void addImage(Guid masterImageId,Stream imageStream,string imageFileName,string imageExtension,string imageTitle,Guid albumId,DynamicContent customRecord)
        {
            LibrariesManager librariesManager = LibrariesManager.GetManager();
            Image image = librariesManager.GetImages().Where(i => i.Id == masterImageId).FirstOrDefault();

            if (image == null)
            {
                //The album post is created as master. The masterImageId is assigned to the master version.
                image = librariesManager.CreateImage(masterImageId);

                //Set the parent album.
                Album album = librariesManager.GetAlbums().Where(i => i.Id == albumId).SingleOrDefault();
                image.Parent = album;

                //Set the properties of the album post.
                image.Title = imageTitle;
                image.DateCreated = DateTime.UtcNow;
                image.PublicationDate = DateTime.UtcNow;
                image.LastModified = DateTime.UtcNow;
                image.UrlName = Regex.Replace(imageTitle.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");
                image.MediaFileUrlName = Regex.Replace(imageFileName.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");

                //Upload the image file.
                // The imageExtension parameter must contain '.', for example '.jpeg'
                librariesManager.Upload(image, imageStream, imageExtension);

                //Save the changes.
                librariesManager.SaveChanges();

                //Publish the Albums item. The live version acquires new ID.
                //var bag = new Dictionary<string, string>();
                //bag.Add("ContentType", typeof(Image).FullName);
                //WorkflowManager.MessageWorkflow(masterImageId, typeof(Image), null, "Publish", false, bag);

                LibrariesManager itemImageManager = LibrariesManager.GetManager();
                var itemImageItem = itemImageManager.GetImages().FirstOrDefault(i => i.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Master);
                if (itemImageItem != null)
                {
                    // This is how we relate an item
                    customRecord.CreateRelation(itemImageItem, "ItemImage");
                }

            }
        }
    }
}
