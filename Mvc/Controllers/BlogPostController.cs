using Sitefinity_Web.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Blogs.Model;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Workflow;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Modules.News;
using Guid = System.Guid;

namespace Sitefinity_Web.Mvc.Controllers
{

    [ControllerToolboxItem(Name = "BlogPostWiget", Title = "Blog Form Widget", SectionName = "Blog Widgets")]
    public class BlogPostController : Controller
    {
        // GET: BlogPost
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult postForm(BlogPostModel post) 
        {

            BlogPostModel postModel = new BlogPostModel();
            postModel.Title = post.Title;
            postModel.Description = post.Description;
            


            BlogsManager blogsManager = BlogsManager.GetManager();
            Blog blog = blogsManager.GetBlogs().Where(b => b.Title == "Blog_Post").FirstOrDefault();


            Guid Id = Guid.NewGuid();

            createBlogPost(Id, post, blog);



            return View("Index", postModel);
        }

        private void createBlogPost(Guid id,BlogPostModel post, Blog blogpost)
        {

            BlogsManager blogsManager = BlogsManager.GetManager();
            BlogPost blogPost = blogsManager.GetBlogPosts().Where(item => item.Title == post.Title).FirstOrDefault();

            string tags = post.Tags;

            string category = post.Category;


            if (blogPost == null)
            {
                //The Blogs item is created as a master. The masterBlogPostId is assigned to the master.
                blogPost = blogsManager.CreateBlogPost(id);

                //Set the parent blog.
                Blog blog = blogsManager.GetBlogs().Where(b => b.Id == blogpost.Id).SingleOrDefault();
                blogPost.Parent = blog;

                //Set the properties of the blog post.                
                blogPost.Title = post.Title;
                blogPost.Content = post.Description;
                blogPost.DateCreated = DateTime.UtcNow;
                blogPost.PublicationDate = DateTime.UtcNow;
                blogPost.LastModified = DateTime.UtcNow;
                blogPost.UrlName = Regex.Replace(blogPost.Title.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");


                //Recompiles and validates the url of the blog.
                blogsManager.RecompileAndValidateUrls(blogPost);



                // adding tagging to the library

                addTags(tags);
                addTaxon(blogPost, tags);


                //adding category to the library

                addCategory(category);

                addCategories(blogPost, category);



                //Save the changes.
                blogsManager.SaveChanges();


                //Publish the Blogs item. The published version acquires new ID.
                var bag = new Dictionary<string, string>();
                bag.Add("ContentType", typeof(BlogPost).FullName);
                WorkflowManager.MessageWorkflow(id, typeof(BlogPost), null, "Publish", false, bag);


            }
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

        public void addTaxon(BlogPost blogpost, string tagname)
        {
            var taxonomyManager = TaxonomyManager.GetManager();
            var Tag = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Taxonomy.Name == "Tags");


            foreach (var Tags in Tag.Where(w => w.Title.ToLower() == tagname.ToLower()))
            {
                if (Tags != null)
                {
                    blogpost.Organizer.AddTaxa("Tags", Tags.Id);

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

        public void addCategories(BlogPost blogpost, string categoryName)
        {
            TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
            var Category = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Taxonomy.Name == "Categories");

            foreach (var categorys in Category.Where(w => w.Title.ToLower() == categoryName.ToLower()))
            {

                if (categorys != null)
                {
                    blogpost.Organizer.AddTaxa("Category", categorys.Id);
                }
            }
        }

    }
}