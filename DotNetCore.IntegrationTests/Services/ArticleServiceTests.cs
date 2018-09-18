using System;
using System.Collections.Generic;
using DotNetCore.Database;
using DotNetCore.Database.Entities;
using DotNetCore.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetCore.Contracts;
using DotNetCore.Exceptions;
using DotNetCore.Enums;
using System.Transactions;

namespace DotNetCore.IntegrationTests.Services
{
    [TestClass]
    public class ArticleServiceTests : BaseIntegrationTest
    {
        private IArticleService articleService;
        private IBlogDbContext blogDbContext;
        private TransactionScope transactionScope;

        [TestInitialize]
        public void OnTestInitialize()
        {
            blogDbContext = CreateBlogDbContext();
            transactionScope = CreateTransactionScope();
            blogDbContext.Create(GetUserList(), true);
            blogDbContext.Create(GetArticleList(blogDbContext), true);
            articleService = new ArticleService(blogDbContext);
        }

        [TestCleanup]
        public void OnTestCleanup()
        {
            if (transactionScope != null)
                transactionScope.Dispose();

            transactionScope = null;
        }

        #region Helpers

        private static IList<Article> GetArticleList(IBlogDbContext blogDbContext)
        {
            var userId = blogDbContext.Set<User>().Where(it => it.UserType == UserType.Publisher).Single().Id;

            return new List<Article>()
            {
                new Article() {
                    Title = "Title1",
                    Body = "Body1",
                    Author = GetUniqueStringValue("author_"),
                    IsPublished = false,
                    PublishDate = null,
                    AddedByUserId = userId,
                },
                new Article() {
                    Title = "Title2",
                    Body = "Body2",
                    Author = GetUniqueStringValue("author_"),
                    IsPublished = false,
                    PublishDate = null,
                    AddedByUserId = userId,
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
                    UserType = UserType.Employee,
                    Password = GetUniqueStringValue(),
                    PasswordSalt = GetUniqueStringValue(),
                },
                new User() {
                    Name = "test_user_2",
                    Email = "test_user_2@gmail.com",
                    UserType = UserType.Publisher,
                    Password = GetUniqueStringValue(),
                    PasswordSalt = GetUniqueStringValue(),
                }
            };
        }

        #endregion

        #region PublishArticle

        [TestMethod]
        public void PublishArticle_Should_Set_IsPublished_To_True_And_PublishDate_To_DateValue()
        {
            var article = blogDbContext.Set<Article>().First();

            articleService.PublishArticle(article.Id);

            var publishedArticle = blogDbContext.GetById<Article>(article.Id);

            Assert.AreEqual(publishedArticle.IsPublished, true);
            Assert.AreEqual(publishedArticle.PublishDate.HasValue, true);
        }


        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void PublishArticle_Should_Throw_EntityNotFoundException_If_Article_Is_Not_Found()
        {
            articleService.PublishArticle(int.MaxValue);

            Assert.Fail();
        }

        #endregion

        #region PublishArticleAsync

        //[TestMethod]
        //public void PublishArticleAsync_Should_Set_IsPublished_To_True_And_PublishDate_To_DateValue()
        //{
        //    var article = blogDbContext.Set<Article>().First();

        //    articleService.PublishArticleAsync(article.Id).Wait();

        //    var publishedArticle = blogDbContext.GetById<Article>(article.Id);

        //    Assert.AreEqual(publishedArticle.IsPublished, true);
        //    Assert.AreEqual(publishedArticle.PublishDate.HasValue, true);
        //}

        //[TestMethod]
        //public void PublishArticleAsync_Should_Throw_InvalidOperationException_If_Article_Is_Already_Published()
        //{
        //    var article = blogDbContext.Set<Article>().First();
        //    try
        //    {
        //        articleService.PublishArticleAsync(article.Id).Wait();
        //        articleService.PublishArticleAsync(article.Id).Wait();
        //    }
        //    catch (AggregateException aggregateException)
        //    {
        //        Assert.IsInstanceOfType(aggregateException.InnerException, typeof(InvalidOperationException));
        //    }
        //}

        //[TestMethod]
        //public void PublishArticleAsync_Should_Throw_EntityNotFoundException_If_Article_Is_Not_Found()
        //{
        //    try
        //    {
        //        articleService.PublishArticleAsync(int.MaxValue).Wait();
        //    }
        //    catch (AggregateException aggregateException)
        //    {
        //        Assert.IsInstanceOfType(aggregateException.InnerException, typeof(EntityNotFoundException));
        //    }
        //}

        #endregion

        #region UnPublishArticle

        [TestMethod]
        public void UnPublishArticle_Should_Set_IsPublished_To_False_And_PublishDate_To_Null()
        {
            var article = blogDbContext.Set<Article>().First();

            articleService.PublishArticle(article.Id);
            articleService.UnPublishArticle(article.Id);

            var unPublishedArticle = blogDbContext.GetById<Article>(article.Id);

            Assert.AreEqual(unPublishedArticle.IsPublished, false);
            Assert.AreEqual(unPublishedArticle.PublishDate.HasValue, false);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void UnPublishArticle_Should_Throw_EntityNotFoundException_If_Article_Is_Not_Found()
        {
            articleService.UnPublishArticle(int.MaxValue);

            Assert.Fail();
        }

        #endregion

        #region UnPublishArticleAsync

        //[TestMethod]
        //public void UnPublishArticleAsync_Should_Set_IsPublished_To_False_And_PublishDate_To_Null()
        //{
        //    var article = blogDbContext.Set<Article>().First();

        //    articleService.PublishArticleAsync(article.Id).Wait();
        //    articleService.UnPublishArticleAsync(article.Id).Wait();

        //    var unPublishedArticle = blogDbContext.GetById<Article>(article.Id);

        //    Assert.AreEqual(unPublishedArticle.IsPublished, false);
        //    Assert.AreEqual(unPublishedArticle.PublishDate.HasValue, false);
        //}

        //[TestMethod]
        //public void UnPublishArticleAsync_Should_Throw_InvalidOperationException_If_Article_Is_Not_Published()
        //{
        //    var article = blogDbContext.Set<Article>().First();

        //    try
        //    {
        //        articleService.PublishArticleAsync(article.Id).Wait();

        //        articleService.UnPublishArticleAsync(article.Id).Wait();

        //        articleService.UnPublishArticleAsync(article.Id).Wait();
        //    }
        //    catch (AggregateException aggregateException)
        //    {
        //        Assert.IsInstanceOfType(aggregateException.InnerException, typeof(InvalidOperationException));
        //    }
        //}

        //[TestMethod]
        //public void UnPublishArticleAsync_Should_Throw_EntityNotFoundException_If_Article_Is_Not_Found()
        //{
        //    try
        //    {
        //        articleService.UnPublishArticleAsync(int.MaxValue).Wait();
        //    }
        //    catch (AggregateException aggregateException)
        //    {
        //        Assert.IsInstanceOfType(aggregateException.InnerException, typeof(EntityNotFoundException));
        //    }
        //}

        #endregion

        #region GetArticleMostLikedQuery

        [TestMethod]
        public void GetArticleMostLikedQuery_Should_Return_Most_Liked_Article()
        {
            var article = blogDbContext.Set<Article>().First();

            var user = blogDbContext.Set<User>().First();

            articleService.SaveArticleFeedback(article.Id, user.Id, ArticleStatus.Like);

            var articleMostLiked = articleService.GetArticleMostLikedQuery().First();
            Assert.IsTrue(articleMostLiked.Count > 0);
        }

        #endregion

        #region SaveArticleFeedback Validate Parameters

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void SaveArticleFeedback_Should_Throw_EntityNotFoundException_If_Article_Is_Not_Found()
        {
            articleService.SaveArticleFeedback(int.MaxValue, 1, ArticleStatus.Like);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveArticleFeedback_Should_Throw_ArgumentException_If_Article_Id_Is_Not_Valid()
        {
            articleService.SaveArticleFeedback(-1, 1, ArticleStatus.Like);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void SaveArticleFeedback_Should_Throw_EntityNotFoundException_If_User_Is_Not_Found()
        {
            articleService.SaveArticleFeedback(1, int.MaxValue, ArticleStatus.Like);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveArticleFeedback_Should_Throw_ArgumentException_If_UserId_Id_Is_Not_Valid()
        {
            articleService.SaveArticleFeedback(1, -1, ArticleStatus.Like);
        }

        #endregion

        #region SaveArticleFeedback 

        [TestMethod]
        public void SaveArticleFeedback_Should_Add_Feedback_Entry()
        {
            var article = blogDbContext.Set<Article>().First();

            var user = blogDbContext.Set<User>().First();

            articleService.SaveArticleFeedback(article.Id, user.Id, ArticleStatus.Like);

            var articleFeedback = blogDbContext.Set<ArticleFeedback>().AsNoTracking().Where(it => it.UserId == user.Id && it.ArticleId == article.Id).Single();

            Assert.IsNotNull(articleFeedback);
            Assert.IsTrue(articleFeedback.FeedbackCount == 1);
            Assert.IsTrue(articleFeedback.ArticleId == article.Id);
            Assert.IsTrue(articleFeedback.UserId == user.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SaveArticleFeedback_Should_Throw_Exception_On_Exceeding_Max_Attempts()
        {
            var article = blogDbContext.Set<Article>().First();

            var user = blogDbContext.Set<User>().First();

            for (int i = 0; i <= articleService.MaxArticleFeedbackAttempts; i++)
            {
                articleService.SaveArticleFeedback(article.Id, user.Id, ArticleStatus.Like);
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
            var article = blogDbContext.Set<Article>().First();

            var user = blogDbContext.Set<User>().First();

            var comments = "didn't like it";

            articleService.SaveArticleComments(article.Id, user.Id, "didn't like it");

            var articleFeedback = blogDbContext.Set<ArticleFeedback>().AsNoTracking().Where(it => it.UserId == user.Id && it.ArticleId == article.Id).Single();

            Assert.IsNotNull(articleFeedback);
            Assert.IsTrue(articleFeedback.Comments == comments);
            Assert.IsTrue(articleFeedback.CommentDate.HasValue);
            Assert.IsTrue(articleFeedback.ArticleId == article.Id);
            Assert.IsTrue(articleFeedback.UserId == user.Id);
        }

        #endregion
    }
}
