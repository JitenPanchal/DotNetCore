using DotNetCore.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Contracts
{
    public interface IArticleService
    {
        void CreateArticle(Article article);
    }
}
