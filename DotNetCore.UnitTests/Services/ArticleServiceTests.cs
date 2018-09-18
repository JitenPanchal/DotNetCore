using DotNetCore.Database;
using DotNetCore.Database.Entities;
using DotNetCore.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetCore.Contracts;
using System;

namespace DotNetCore.UnitTests
{
    [TestClass]
    public class ArticleServiceTests : BaseUnitTest
    {
        IArticleService articleService;

        [TestInitialize]
        public void OnTestInitialize()
        {
            articleService = CreateArticleService();
            articleService.Create<Article>(GetArticleList(), true);
        }

        [TestCleanup]
        public void OnTestCleanup()
        {
            articleService.DbContext.Database.EnsureDeleted();
        }

        private static ArticleService CreateArticleService()
        {
            return new ArticleService(CreateBlogDbContext());
        }

        private static List<Article> GetArticleList()
        {
            return new List<Article>()
            {
                new Article() {
                    Title = "Title1",
                    Body = "Body1",
                    IsPublished = false,
                    PublishDate = null
                },
                new Article() {
                    Title = "Title2",
                    Body = "Body2",
                    IsPublished = false,
                    PublishDate = null
                }
            };
        }

        [TestMethod]
        public void CreateArticle_should_add_article_to_database()
        {
            articleService.Create(new Article()
            {
                Title = "Title",
                Body = "Body",
                IsPublished = false,
                PublishDate = null
            }, true);

            IArticleService tmpArticleService = CreateArticleService();

            Assert.IsTrue(tmpArticleService.DbContext.Set<Article>().Count() == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PublishArticle_should_throw_exception_when_invalid_article_id_is_provided()
        {
            articleService.PublishArticle(int.MinValue);
        }

        [TestMethod]
        public void PublishArticle_should_update_publish_date_and_is_published_fields()
        {
            var article = new Article()
            {
                Title = "Title",
                Body = "Body",
                IsPublished = false,
                PublishDate = null
            };

            articleService.Create(article, true);

            articleService.PublishArticle(article.Id);

            var updatedArticle = articleService.DbContext.GetById<Article>(article.Id);

            Assert.AreEqual(DateTime.UtcNow.Date , updatedArticle.PublishDate.Value.Date);
            Assert.AreEqual(true, updatedArticle.IsPublished);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnPublishArticle_should_throw_exception_when_invalid_article_id_is_provided()
        {
            articleService.UnPublishArticle(int.MinValue);
        }

        [TestMethod]
        public void UnPublishArticle_should_update_publish_date_and_is_published_fields()
        {
            var article = new Article()
            {
                Title = "Title",
                Body = "Body",
                IsPublished = false,
                PublishDate = null
            };

            articleService.Create(article, true);

            articleService.PublishArticle(article.Id);

            articleService.UnPublishArticle(article.Id);

            var updatedArticle = articleService.DbContext.GetById<Article>(article.Id);

            Assert.AreEqual(null, updatedArticle.PublishDate);
            Assert.AreEqual(false, updatedArticle.IsPublished);
        }
    }
}