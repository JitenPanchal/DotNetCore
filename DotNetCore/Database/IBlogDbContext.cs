using DotNetCore.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Database
{
    public interface IBlogDbContext : IDbContext, IDisposable
    {
        DbSet<Article> Articles { get; set; }
        DbSet<ArticleFeedback> ArticleFeedbacks { get; set; }
    }
}