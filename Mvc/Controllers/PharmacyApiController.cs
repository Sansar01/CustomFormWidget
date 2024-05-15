using Newtonsoft.Json;
using Sitefinity_Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Workflow;
using Guid = System.Guid;
using Telerik.Windows.Documents.RichTextBoxCommands;
using Telerik.Sitefinity.Modules.Libraries;
using Album = Telerik.Sitefinity.Libraries.Model.Album;
using System.IO;
using Telerik.Sitefinity.RelatedData;
using Image = Telerik.Sitefinity.Libraries.Model.Image;
using Azure;
using System.Net;
using AngleSharp.Dom;
using static ServiceStack.Host.HttpListener.ListenerRequest;


namespace Sitefinity_Web.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "PharmacyApiWidget", Title = "Pharmacy api widget", SectionName = "Pharmacy Widgets")]
    public class PharmacyApiController : Controller
    {
        Guid albumId;
        string albumTitle = string.Empty;

        // GET: PharmacyApi
        Uri apiUrl = new Uri("https://www.pharmacyitk.com.au/wp-json/wp/v2/posts");
        public ActionResult Index()
        {
            List<BlogPostData> dataObj = new List<BlogPostData>();

            HttpClient client = new HttpClient();
            client.BaseAddress = apiUrl;

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                dataObj = JsonConvert.DeserializeObject<List<BlogPostData>>(jsonResponse);

            }
            retriveData(dataObj);
            return View(dataObj);
        }


        private ActionResult retriveData(List<BlogPostData> dataObj)
        {

            BlogsManager blogsManager = BlogsManager.GetManager();
            Blog blog = blogsManager.GetBlogs().Where(b => b.Title == "Blogapi").FirstOrDefault();

            foreach (var post in dataObj)
            {
                Guid Id = Guid.NewGuid();
                //addImage(post.id);
                createBlogPost(Id, post, blog);
            }
            return View("Index");
        }

        private void createBlogPost(Guid id, BlogPostData post, Blog blogpost)
        {

            BlogsManager blogsManager = BlogsManager.GetManager();
            BlogPost blogPost = blogsManager.GetBlogPosts().Where(item => item.Title == post.title.rendered).FirstOrDefault();

            //string tags = post.Tags;

            //string category = post.Category;


            if (blogPost == null)
            {
                //The Blogs item is created as a master. The masterBlogPostId is assigned to the master.
                blogPost = blogsManager.CreateBlogPost(id);

                //Set the parent blog.
                Blog blog = blogsManager.GetBlogs().Where(b => b.Id == blogpost.Id).SingleOrDefault();
                blogPost.Parent = blog;

                //Set the properties of the blog post.                
                blogPost.Title = post.title.rendered;
                blogPost.Content = post.content.rendered;
                blogPost.DateCreated = DateTime.UtcNow;
                blogPost.PublicationDate = DateTime.UtcNow;
                blogPost.LastModified = DateTime.UtcNow;
                blogPost.UrlName = Regex.Replace(blogPost.Title.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");


                //Recompiles and validates the url of the blog.
                blogsManager.RecompileAndValidateUrls(blogPost);

                
                addTags(post.id, blogPost);
                addCategory(post.id, blogPost);
                getImage(post.id, blogPost);


                //Save the changes.
                blogsManager.SaveChanges();


                //Publish the Blogs item. The published version acquires new ID.
                var bag = new Dictionary<string, string>();
                bag.Add("ContentType", typeof(BlogPost).FullName);
                WorkflowManager.MessageWorkflow(id, typeof(BlogPost), null, "Publish", false, bag);


            }
        }

        public void addTags(int tagId, BlogPost blogPost)
        {
            List<TagsPostApi> tagObj = new List<TagsPostApi>();
            string apiUrl = "https://www.pharmacyitk.com.au/wp-json/wp/v2/tags?post=" + tagId;

            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                tagObj = JsonConvert.DeserializeObject<List<TagsPostApi>>(jsonResponse);

            }
            var taxonomyManager = TaxonomyManager.GetManager();
            var Tags = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Taxonomy.Name == "Tags");


            foreach (var tagName in tagObj)
            {

                string tags = tagName.Name;

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

                if (Tags != null)
                {
                    addTaxon(tagObj, blogPost, tags);
                }

            }  
        }

        public void addTaxon(List<TagsPostApi> ListTagsObj, BlogPost blogPost,string tags)
        {
            var taxonomyManager = TaxonomyManager.GetManager();
            var Tags = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Taxonomy.Name == "Tags");

  
                foreach (var ItemTags in Tags.Where(w => w.Title.ToLower() == tags.ToLower()))
                {
                    if (ItemTags != null)
                    {
                        blogPost.Organizer.AddTaxa("Tags", ItemTags.Id);

                    }
                }
        }

        public void addCategory(int categoryId, BlogPost blogPost)
        {
            List<CategoryPostApi> categoryObj = new List<CategoryPostApi>();
            string apiUrl = "https://www.pharmacyitk.com.au/wp-json/wp/v2/categories?post=" + categoryId;

            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                categoryObj = JsonConvert.DeserializeObject<List<CategoryPostApi>>(jsonResponse);

            }

            var taxonomyManager = TaxonomyManager.GetManager();

            //Get the Categories taxonomy
            var categoryTaxonomy = taxonomyManager.GetTaxonomies<HierarchicalTaxonomy>().SingleOrDefault(s => s.Name == "Categories");

            foreach (var categoryName in categoryObj)
            {
                string category = categoryName.name;

                    if (categoryTaxonomy == null) return;

                    //Create a new HierarchicalTaxon
                    var taxon = taxonomyManager.CreateTaxon<HierarchicalTaxon>();

                    //Associate the item with the hierarchical taxonomy
                    taxon.Taxonomy = categoryTaxonomy;

                    taxon.Name = Regex.Replace(categoryName.name.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");
                    taxon.Title = categoryName.name;
                    taxon.UrlName = Regex.Replace(categoryName.name.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");


                    //HierarchicalTaxon parentCategory = new HierarchicalTaxon();

                    //Check if the parent has been set
                    //if (parentCategory != null)
                    //{
                    //    taxon.Parent = parentCategory;
                    //}

                    //Add it to the list
                    categoryTaxonomy.Taxa.Add(taxon);

                    taxonomyManager.SaveChanges();

                if (categoryTaxonomy != null)
                {
                    addCategories(categoryObj, blogPost, category);
                }

            }

           
        }
        public void addCategories(List<CategoryPostApi> categoryObj, BlogPost blogPost,string category)
        {
            TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
            var Category = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Taxonomy.Name == "Categories");

           
                foreach (var categoryItem in Category.Where(i => i.Name.ToLower() == category.ToLower()))
                {
                    if (categoryItem != null)
                    {
                        blogPost.Organizer.AddTaxa("Category", categoryItem.Id);
                    }
                }
        }
   
        public void getImage(int imageId,BlogPost blogPost)
        {
            List<ImagePostData.ImageApi> imageObj = new List<ImagePostData.ImageApi>();
            string apiUrl = "https://www.pharmacyitk.com.au/wp-json/wp/v2/media?parent=" + imageId;

            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                imageObj = JsonConvert.DeserializeObject<List<ImagePostData.ImageApi>>(jsonResponse);

            }
            createImage(imageObj,blogPost);
        }

        public void addAlbum()
        {
            //gets an isntance of the LibrariesManager
            var manager = LibrariesManager.GetManager();
            Album albumManager = manager.GetAlbums().Where(a => a.Title == "BlogPostImage").FirstOrDefault();

            if (albumManager == null)
            {
                //creates an image album(library)
                var imagesAlbum = manager.CreateAlbum();
                imagesAlbum.Title = "BlogPostImage1";
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

        public void createImage(List<ImagePostData.ImageApi> imageObj,BlogPost blogPost)
        {
            addAlbum();
           foreach(var Imagefile in imageObj)
            {
                if (Imagefile.guid.rendered != null)
                {


                    // Generate a new GUID for the image
                    System.Guid masterImageId = Guid.NewGuid();

                    HttpClient httpClient = new HttpClient();
                    
                    HttpResponseMessage response = httpClient.GetAsync(Imagefile.media_details.sizes.medium.source_url).Result;


                    WebClient client = new WebClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.Headers.Add("user-agent", "user agent string");
                    string imageUrl = Imagefile.guid.rendered;
                    byte[] imageBytes = client.DownloadData(imageUrl);

                    MemoryStream memoryStream = new MemoryStream(imageBytes);

                    memoryStream.Position = 0;


                    Stream imageStream = memoryStream;

                   

                    if (response.IsSuccessStatusCode)
                    {

                        // Obtain the image stream from the uploaded file
                        imageStream = response.Content.ReadAsStreamAsync().Result;
                    }

                    // Obtain the image file name
                    string imageFileName = Imagefile.media_details.file;

                    // Obtain the image extension from the file name
                    string imageExtension = System.IO.Path.GetExtension(imageFileName);


                    string imageTitle = System.IO.Path.GetFileNameWithoutExtension(Imagefile.title.rendered);


                    addImage(masterImageId,imageStream, imageFileName, imageExtension, imageTitle, albumId,blogPost);

                }
            }
        }
        public void addImage(Guid masterImageId,Stream imageStream,string imageFileName, string imageExtension, string imageTitle, Guid albumId,BlogPost blogPost)
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
                var itemImageItem = librariesManager.GetImages().FirstOrDefault(i => i.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Master);
                if (itemImageItem != null)
                {
                    // This is how we relate an item
                    blogPost.CreateRelation(itemImageItem, "Images");
                }

            }
        }
    }
}