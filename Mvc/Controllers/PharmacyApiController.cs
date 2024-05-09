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
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Workflow;
using Guid = System.Guid;

namespace Sitefinity_Web.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "PharmacyApiWidget", Title = "Pharmacy api widget", SectionName = "Pharmacy Widgets")]
    public class PharmacyApiController : Controller
    {
        // GET: PharmacyApi
        public ActionResult Index()
         {
            return View();
        }

        [HttpPost]
        public ActionResult ButtonSubmit(string Title)
        {
            getApiData();
            return View("Index");
        }
        private async Task<List<BlogPostData>> getApiData()
        {
            string apiUrl = "https://www.pharmacyitk.com.au/wp-json/wp/v2/posts";
            List<BlogPostData> dataObj = new List<BlogPostData>();

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync(); ;
                dataObj = JsonConvert.DeserializeObject<List<BlogPostData>>(jsonResponse);

            }
            return retriveData(dataObj);
        }

        private List<BlogPostData> retriveData(List<BlogPostData> dataObj)
        {

            BlogsManager blogsManager = BlogsManager.GetManager();
            Blog blog = blogsManager.GetBlogs().Where(b => b.Title == "BlogPostApi").FirstOrDefault();

            foreach (var post in dataObj)
            {

                int yourIntId = post.id;


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

                //Save the changes.
                blogsManager.SaveChanges();


                //Publish the Blogs item. The published version acquires new ID.
                var bag = new Dictionary<string, string>();
                bag.Add("ContentType", typeof(BlogPost).FullName);
                WorkflowManager.MessageWorkflow(id, typeof(BlogPost), null, "Publish", false, bag);


            }
        }

    }
}