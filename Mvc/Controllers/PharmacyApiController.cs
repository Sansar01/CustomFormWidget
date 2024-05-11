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

namespace Sitefinity_Web.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "PharmacyApiWidget", Title = "Pharmacy api widget", SectionName = "Pharmacy Widgets")]
    public class PharmacyApiController : Controller
    {
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
            return View();
        }


        private List<BlogPostData> retriveData(List<BlogPostData> dataObj)
        {

            BlogsManager blogsManager = BlogsManager.GetManager();
            Blog blog = blogsManager.GetBlogs().Where(b => b.Title == "Blog_Api").FirstOrDefault();

            foreach (var post in dataObj)
            {
                Guid Id = Guid.NewGuid();
                createBlogPost(Id, post, blog);
            }
            throw new NotImplementedException();
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

                addTags(post.id,blogPost);

                //Save the changes.
                blogsManager.SaveChanges();


                //Publish the Blogs item. The published version acquires new ID.
                var bag = new Dictionary<string, string>();
                bag.Add("ContentType", typeof(BlogPost).FullName);
                WorkflowManager.MessageWorkflow(id, typeof(BlogPost), null, "Publish", false, bag);


            }
        }

        public void addTags(int tagId,BlogPost blogPost)
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
                if(tagName.Taxonomy =="post_tag")
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
                }
               
            }
                           
            if(Tags!=null)
            {
                addTaxon(tagObj,blogPost);
            }
        }

        public void addTaxon(List<TagsPostApi> ListTagsObj,BlogPost blogPost)
        {
            var taxonomyManager = TaxonomyManager.GetManager();
            var Tags = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Taxonomy.Name == "Tags");

            foreach (var getTag in ListTagsObj)
            {
                foreach (var ItemTags in Tags.Where(w => w.Title.ToLower() == getTag.Name.ToLower()))
                {
                    if (ItemTags != null)
                    {
                        blogPost.Organizer.AddTaxa("Tags", ItemTags.Id);

                    }
                }
            }
        }
    }
}