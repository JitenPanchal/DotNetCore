using DotNetCore.Database;
using DotNetCore.Database.Entities;
using DotNetCore.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetCore.Contracts;
using System;
using DotNetCore.Exceptions;
using DotNetCore.Enums;

namespace DotNetCore.UnitTests
{
    [TestClass]
    public class ArticleServiceTests : BaseUnitTest
    {
        private IMembershipService membershipService;
        IArticleService articleService;
        IBlogDbContext blogDbContext;

        [TestInitialize]
        public void OnTestInitialize()
        {
            blogDbContext = CreateInMemoryBlogDbContext();
            membershipService = new MembershipService(blogDbContext);
            articleService = new ArticleService(blogDbContext, membershipService);
            articleService.Create(GetArticleList(), true);
            articleService.Create(GetUserList(), true);
        }

        [TestCleanup]
        public void OnTestCleanup()
        {
            blogDbContext.Database.EnsureDeleted();
            blogDbContext.Dispose();
            articleService = null;
        }

        #region Helpers

        private static IList<Article> GetArticleList()
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

        private static IList<User> GetUserList()
        {
            return new List<User>()
            {
                new User() {
                    Name = "test_user_1",
                    Email = "test_user_1@gmail.com",
                    UserType = UserType.Employee

                },
                new User() {
                    Name = "test_user_2",
                    Email = "test_user_2@gmail.com",
                    UserType = UserType.Publisher,
                }
            };
        }

        #endregion

        #region Publish Article

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PublishArticle_should_throw_exception_when_invalid_article_id_is_provided()
        {
            articleService.PublishArticle(int.MinValue);
        }

        [TestMethod]
        public void PublishArticle_should_update_publish_date_and_is_published_fields()
        {
            var article = blogDbContext.Set<Article>().FirstOrDefault();

            articleService.PublishArticle(article.Id);

            var updatedArticle = articleService.GetById<Article>(article.Id);

            Assert.AreEqual(DateTime.UtcNow.Date, updatedArticle.PublishDate.Value.Date);
            Assert.AreEqual(true, updatedArticle.IsPublished);
        }

        #endregion

        #region UnPublish Article

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnPublishArticle_should_throw_exception_when_invalid_article_id_is_provided()
        {
            articleService.UnPublishArticle(int.MinValue);
        }

        [TestMethod]
        public void UnPublishArticle_should_update_publish_date_and_is_published_fields()
        {
            var article = blogDbContext.Set<Article>().First();

            articleService.PublishArticle(article.Id);

            articleService.UnPublishArticle(article.Id);

            var updatedArticle = articleService.GetById<Article>(article.Id);

            Assert.AreEqual(null, updatedArticle.PublishDate);
            Assert.AreEqual(false, updatedArticle.IsPublished);
        }

        #endregion

        #region SaveArticleFeedback Validate Parameters

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void SaveArticleFeedback_Should_Throw_EntityNotFoundException_If_Article_Is_Not_Found()
        {
            articleService.SaveArticleFeedback(int.MaxValue, ArticleStatus.Like);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveArticleFeedback_Should_Throw_ArgumentException_If_Article_Id_Is_Not_Valid()
        {
            articleService.SaveArticleFeedback(-1, ArticleStatus.Like);
        }

        #endregion

        #region SaveArticleFeedback 

        [TestMethod]
        public void SaveArticleFeedback_Should_Add_Feedback_Entry()
        {
            var article = blogDbContext.Set<Article>().AsNoTracking().FirstOrDefault();

            articleService.SaveArticleFeedback(article.Id, ArticleStatus.Like);

            var articleFeedback = articleService.GetArticleFeeback(article.Id, membershipService.CurrentUser.Id).FirstOrDefault();

            Assert.IsNotNull(articleFeedback);
            Assert.IsTrue(articleFeedback.FeedbackCount == 1);
            Assert.IsTrue(articleFeedback.ArticleId == article.Id);
            Assert.IsTrue(articleFeedback.UserId == membershipService.CurrentUser.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SaveArticleFeedback_Should_Throw_Exception_On_Exceeding_Max_Attempts()
        {
            var article = blogDbContext.Set<Article>().FirstOrDefault();

            for (int i = 0; i <= articleService.MaxArticleFeedbackAttempts; i++)
            {
                articleService.SaveArticleFeedback(article.Id, ArticleStatus.Like);
            }
        }

        #endregion

        #region SaveArticleComments Validate Parameters

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void SaveArticleComments_Should_Throw_EntityNotFoundException_If_Article_Is_Not_Found()
        {
            articleService.SaveArticleComments(int.MaxValue, 1, "Like it ?");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveArticleComments_Should_Throw_ArgumentException_If_Article_Id_Is_Not_Valid()
        {
            articleService.SaveArticleComments(-1, 1, "Like it ?");
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void SaveArticleComments_Should_Throw_EntityNotFoundException_If_User_Is_Not_Found()
        {
            articleService.SaveArticleComments(1, int.MaxValue, "Like it ?");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveArticleComments_Should_Throw_ArgumentException_If_UserId_Id_Is_Not_Valid()
        {
            articleService.SaveArticleComments(1, -1, "Like it ?");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveArticleComments_Should_Throw_ArgumentException_If_Comments_Is_Null()
        {
            articleService.SaveArticleComments(1, -1, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveArticleComments_Should_Throw_ArgumentException_If_Comments_Is_Empty()
        {
            articleService.SaveArticleComments(1, -1, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveArticleComments_Should_Throw_ArgumentException_If_Comments_Is_Contains_Only_WhiteSpace()
        {
            articleService.SaveArticleComments(1, -1, "              ");
        }

        #endregion

        #region SaveArticleComments

        [TestMethod]
        public void SaveArticleComments_Should_Save_Comments_And_Set_CommentsDate()
        {
            var comments = "didn't like it";

            var article = blogDbContext.Set<Article>().AsNoTracking().FirstOrDefault();
            var user = blogDbContext.Set<User>().AsNoTracking().FirstOrDefault();

            articleService.SaveArticleComments(article.Id, user.Id, comments);

            var articleFeedback = articleService.GetArticleFeeback(article.Id, user.Id).FirstOrDefault();

            Assert.IsNotNull(articleFeedback);
            Assert.IsTrue(articleFeedback.Comments == comments);
            Assert.IsTrue(articleFeedback.CommentDate.HasValue);
            Assert.IsTrue(articleFeedback.ArticleId == article.Id);
            Assert.IsTrue(articleFeedback.UserId == user.Id);
        }

        #endregion
    }
}