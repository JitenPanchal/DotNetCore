using DotNetCore.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Transactions;

namespace DotNetCore.IntegrationTests
{
    [TestClass()]
    public abstract class BaseIntegrationTest
    {
        private static string testDatabaseConnection = null;

        public static readonly ILoggerFactory loggerFactory = new LoggerFactory()
            .AddDebug((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name))
            .AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name));

        private static IConfiguration GetConfig()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            return configuration;
        }

        private static string GetConnectionString()
        {
            var config = GetConfig();
            return config.GetConnectionString("connectionString");
        }

        private static DbContextOptions<DbContext> GetDbContextOptions(string connectionString)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DbContext>();

            dbContextOptionsBuilder.UseSqlServer(connectionString);
            dbContextOptionsBuilder.UseLoggerFactory(loggerFactory);

            return dbContextOptionsBuilder.Options;
        }

        protected static BlogDbContext CreateBlogDbContext()
        {
            return new BlogDbContext(GetDbContextOptions(testDatabaseConnection));
        }

        [AssemblyInitialize()]
        public static void AssemblyInitialize(TestContext context)
        {
            testDatabaseConnection = string.Format(GetConnectionString(), Guid.NewGuid().ToString("N"));

            using (var blogDbContext = CreateBlogDbContext())
            {
                blogDbContext.Database.EnsureCreated();
            }
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            using (var blogDbContext = CreateBlogDbContext())
            {
                blogDbContext.Database.EnsureDeleted();
            }
        }

        protected static string GetUniqueStringValue(string value = "test_")
        {
            return string.Format("{0}{1}", value, Guid.NewGuid().ToString("N"));
        }

        protected static TransactionScope CreateTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
