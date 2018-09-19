using DotNetCore.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DotNetCore.Database
{
    public interface IBlogDbContext : IDbContext, IDisposable
    {
        DbSet<Article> Articles { get; set; }

        DbSet<ArticleFeedback> ArticleFeedbacks { get; set; }

        DbSet<User> Users { get; set; }
    }
}