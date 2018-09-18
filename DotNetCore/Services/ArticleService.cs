using DotNetCore.Contracts;
using DotNetCore.Database;
using DotNetCore.Database.Entities;
using DotNetCore.Enums;
using DotNetCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Services
{
    public class ArticleService : BaseService, IArticleService
    {
        const int MaxArticleFeedbackAttempts = 3;

        #region Error Messages

        const string InvalidArticleFeedbackOperation = "User has exceeded max feedback attempts";

        #endregion

        public ArticleService(IBlogDbContext blogDbContext) : base(blogDbContext)
        {
        }

        public virtual void PublishArticle(int articleId, bool saveChanges = false)
        {
            if (articleId <= 0)
                throw new ArgumentException(nameof(articleId));

            var article = DbContext.GetById<Article>(articleId, false, true);

            article.PublishDate = (DateTime?)DateTime.UtcNow;
            article.IsPublished = true;

            Update(article, saveChanges);
        }

        public virtual void UnPublishArticle(int articleId, bool saveChanges = true)
        {
            if (articleId <= 0)
                throw new ArgumentException(nameof(articleId));

            var article = DbContext.GetById<Article>(articleId, false, true);

            article.PublishDate = null;
            article.IsPublished = false;

            Update(article, saveChanges);
        }

        public virtual void SaveArticleFeedback(int articleId, int userId, ArticleStatus articleStatus)
        {
            if (articleId <= 0)
                throw new ArgumentException(nameof(articleId));

            if (userId <= 0)
                throw new ArgumentException(nameof(userId));

            // validate article
            var article = DbContext.GetById<Article>(articleId, false, true);

            // validate user
            var user = DbContext.GetById<User>(userId, false, true);

            var articleFeedback = (GetArticleFeeback(articleId, userId)).FirstOrDefault();

            articleFeedback = SetArticleFeedbackFields(articleStatus, article, user, articleFeedback);

            DbContext.SaveChanges();
        }

        public virtual IQueryable<ArticleFeedback> GetArticleFeeback(int articleId, int userId)
        {
            return from item in DbContext.Set<ArticleFeedback>()
                   where item.ArticleId == articleId && item.UserId == userId
                   select item;
        }

        public virtual void SaveArticleComments(int articleId, int userId, string comments)
        {
            if (articleId <= 0)
                throw new ArgumentException(nameof(articleId));

            if (userId <= 0)
                throw new ArgumentException(nameof(userId));

            if (string.IsNullOrWhiteSpace(comments))
                throw new ArgumentException(nameof(comments));

            // validate article
            var article = DbContext.GetById<Article>(articleId, false, true);

            // validate user
            var user = DbContext.GetById<User>(userId, false, true);

            var articleFeedback = (GetArticleFeeback(articleId, userId)).FirstOrDefault();

            articleFeedback = SetArticleCommentsFields(comments, article, user, articleFeedback);

            DbContext.SaveChanges();
        }

        public virtual IQueryable<ArticleMostLiked> GetArticleMostLikedQuery()
        {
            var countQuery = (from it in DbContext.Set<ArticleFeedback>()
                              group it by it.ArticleId
                             into g
                              orderby g.Count() descending
                              select new { ArticleId = g.Key, Count = g.Count() }).Take(1);

            var query = from it in DbContext.Set<Article>().AsNoTracking()
                        join tmp in countQuery on it.Id equals tmp.ArticleId
                        join user in DbContext.Set<User>().AsNoTracking() on it.AddedByUserId equals user.Id
                        select new ArticleMostLiked()
                        {
                            Id = it.Id,
                            Title = it.Title,
                            Body = it.Body,
                            Author = it.Author,
                            PublishDate = it.PublishDate,
                            IsPublished = it.IsPublished,
                            Count = tmp.Count,
                            AddedByUserName = user.Name
                        };

            return query;
        }

        public virtual IQueryable<ArticleStatResponse> GetArticlesWithStatQuery(ArticlesSortBy articlesSortBy)
        {
            var query = GetArticlesWithStatQuery();

            switch (articlesSortBy)
            {
                case ArticlesSortBy.MostLikes:
                    query = query.OrderByDescending(it => it.LikeCount);
                    break;
                case ArticlesSortBy.MostRecents:
                    query = query.OrderByDescending(it => it.PublishDate);
                    break;
            }

            return query;
        }

        public virtual IQueryable<ArticleStatResponse> GetMyArticlesWithStatQuery(string username, ArticlesSortBy articlesSortBy)
        {
            var query = GetArticlesWithStatQuery().Where(it => it.Author == username);

            switch (articlesSortBy)
            {
                case ArticlesSortBy.MostLikes:
                    query = query.OrderByDescending(it => it.LikeCount);
                    break;
                case ArticlesSortBy.MostRecents:
                    query = query.OrderByDescending(it => it.PublishDate);
                    break;
            }

            return query;
        }

        public virtual IQueryable<ArticleStatResponse> GetArticleWithStatQuery(int articleId)
        {
            var query = GetArticlesWithStatQuery().Where(it => it.Id == articleId);

            return query;
        }

        public virtual IQueryable<ArticleStatResponse> GetArticlesWithStatQuery()
        {
            var likeQuery = GetArticleStatQuery(ArticleStatus.Like);
            var unlikeQuery = GetArticleStatQuery(ArticleStatus.UnLike);
            var noneQuery = GetArticleStatQuery(ArticleStatus.None);

            var countLikeQuery = (
                from it in likeQuery
                group it by it.ArticleId
                into g
                orderby g.Count() descending
                select new { ArticleId = g.Key, Count = g.Count() }
            );

            var countUnLikeQuery = (
                from it in unlikeQuery
                group it by it.ArticleId
                into g
                orderby g.Count() descending
                select new { ArticleId = g.Key, Count = g.Count() }
            );

            var countNoneQuery = (
                from it in noneQuery
                group it by it.ArticleId
                into g
                orderby g.Count() descending
                select new { ArticleId = g.Key, Count = g.Count() }
            );



            var query =
                from item in DbContext.Set<Article>()
                join statLikeItem in countLikeQuery on item.Id equals statLikeItem.ArticleId into likeTemp
                join statUnLikeItem in countUnLikeQuery on item.Id equals statUnLikeItem.ArticleId into unLikeTemp
                join statNoneItem in countNoneQuery on item.Id equals statNoneItem.ArticleId into noneTemp
                from likeTempItem in likeTemp.DefaultIfEmpty()
                from unLikeTempItem in unLikeTemp.DefaultIfEmpty()
                from noneTempItem in noneTemp.DefaultIfEmpty()
                select new ArticleStatResponse()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Body = item.Body,
                    Author = item.Author,
                    PublishDate = item.PublishDate,
                    IsPublished = item.IsPublished,
                    LikeCount = likeTempItem.Count,
                    UnLikeCount = unLikeTempItem.Count,
                    NoneCount = noneTempItem.Count,
                };

            return query;
        }

        public virtual IQueryable<ArticleFeedback> GetArticleStatQuery(ArticleStatus status)
        {
            var query = from it in DbContext.Set<ArticleFeedback>()
                        where it.Status == status
                        select it;
            return query;
        }

        #region Helpers

        private ArticleFeedback SetArticleFeedbackFields(ArticleStatus articleStatus, Article article, User user, ArticleFeedback articleFeedback)
        {
            if (articleFeedback == null)
            {
                articleFeedback = new ArticleFeedback();
                DbContext.Set<ArticleFeedback>().Add(articleFeedback);
            }

            articleFeedback.FeedbackCount++;

            // check max no. of feedback attempts
            if (articleFeedback.FeedbackCount > MaxArticleFeedbackAttempts)
                throw new InvalidOperationException(InvalidArticleFeedbackOperation);

            articleFeedback.Article = article;
            articleFeedback.User = user;
            articleFeedback.Status = articleStatus;
            articleFeedback.FeedbackDate = DateTime.UtcNow;

            return articleFeedback;
        }

        private static void SetPublishArticleFields(Article article, bool publish)
        {
            article.PublishDate = publish ? (DateTime?)DateTime.UtcNow : null;
            article.IsPublished = publish;
        }

        private ArticleFeedback SetArticleCommentsFields(string comments, Article article, User user, ArticleFeedback articleFeedback)
        {
            if (articleFeedback == null)
            {
                articleFeedback = new ArticleFeedback();
                DbContext.Set<ArticleFeedback>().Add(articleFeedback);
            }

            articleFeedback.Article = article;
            articleFeedback.User = user;
            articleFeedback.Comments = comments;
            articleFeedback.CommentDate = DateTime.UtcNow;
            return articleFeedback;
        }

        #endregion
    }
}