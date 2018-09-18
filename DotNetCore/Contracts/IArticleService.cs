using DotNetCore.Database.Entities;
using DotNetCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Contracts
{
    public interface IArticleService : IBaseService
    {
        void PublishArticle(int articleId, bool saveChanges = true);

        void UnPublishArticle(int articleId, bool saveChanges = true);

        void SaveArticleFeedback(int articleId, int userId, ArticleStatus articleStatus);

        IQueryable<ArticleFeedback> GetArticleFeeback(int articleId, int userId);

        void SaveArticleComments(int articleId, int userId, string comments);
    }
}
