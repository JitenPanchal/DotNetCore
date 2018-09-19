using DotNetCore.Database.Entities;
using DotNetCore.Enums;
using DotNetCore.Models;
using DotNetCore.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Contracts
{
    public interface IArticleService : IBaseService
    {
        int MaxArticleFeedbackAttempts { get; }

        void PublishArticle(int articleId, bool saveChanges = true);

        void UnPublishArticle(int articleId, bool saveChanges = true);

        void SaveArticleFeedback(int articleId, int userId, ArticleStatus articleStatus);

        IQueryable<ArticleFeedback> GetArticleFeeback(int articleId, int userId);

        void SaveArticleComments(int articleId, int userId, string comments);

        IQueryable<ArticleMostLiked> GetArticleMostLikedQuery();

        IQueryable<ArticleStatResponse> GetArticlesWithStatQuery(ArticlesSortBy articlesSortBy);

        IQueryable<ArticleStatResponse> GetMyArticlesWithStatQuery(string username, ArticlesSortBy articlesSortBy);

        IQueryable<ArticleStatResponse> GetArticleWithStatQuery(int articleId);

        IQueryable<ArticleStatResponse> GetArticlesWithStatQuery();

        IQueryable<ArticleFeedback> GetArticleStatQuery(ArticleStatus status);

        IQueryable<Article> GetArticles(ArticlesSortBy articlesSortBy);
    }
}
