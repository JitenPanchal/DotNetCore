using DotNetCore.Database;
using DotNetCore.Database.Entities;
using DotNetCore.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetCore.UnitTests
{
    [TestClass]
    public class ArticleServiceTests : BaseUnitTest
    {
        ArticleService articleService;

        [TestInitialize]
        public void OnTestInitialize()
        {
            articleService = new ArticleService(CreateBlogDbContext());
        }

        [TestCleanup]
        public void OnTestCleanup() { }


        [TestMethod]
        public void CreateArticle_should_add_article_to_database()
        {
            var blogDbContext = CreateBlogDbContext();

            blogDbContext.Articles.Add(new Article()
            {
                Title = "Title",
                Body = "Body",
                IsPublished = false,
                PublishDate = null
            });

            blogDbContext.SaveChanges();

            using (IBlogDbContext x = CreateBlogDbContext())
            {
                Assert.IsTrue(blogDbContext.Articles.Count() == 1);
            }
        }
    }
}