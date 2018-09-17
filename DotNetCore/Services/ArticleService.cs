using DotNetCore.Contracts;
using DotNetCore.Database;
using DotNetCore.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IBlogDbContext _blogDbContext;

        public ArticleService(IBlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        public void CreateArticle(Article article)
        {
            _blogDbContext.Articles.Add(article);
            _blogDbContext.SaveChanges();
        }

    }
}
