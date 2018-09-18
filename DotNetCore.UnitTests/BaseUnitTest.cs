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
        protected static IBlogDbContext CreateBlogDbContext()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DbContext>();

            dbContextOptionsBuilder.UseInMemoryDatabase(Constants.Database.BlogContextDbName, null);

            return new BlogDbContext(dbContextOptionsBuilder.Options);
        }
    }
}
