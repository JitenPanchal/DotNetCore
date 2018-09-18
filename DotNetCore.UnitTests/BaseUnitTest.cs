using DotNetCore.Database;
using DotNetCore.Database.Entities;
using DotNetCore.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetCore.UnitTests
{
    public abstract class BaseUnitTest
    {
        protected static IBlogDbContext CreateInMemoryBlogDbContext()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DbContext>();

            dbContextOptionsBuilder.UseInMemoryDatabase(nameof(BlogDbContext), null);

            return new BlogDbContext(dbContextOptionsBuilder.Options);
        }
    }
}