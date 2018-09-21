using DotNetCore.ApiIntegrationTests.Database;
using DotNetCore.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Transactions;
using System.Web;

namespace DotNetCore.ApiIntegrationTests
{
    [TestClass()]
    public abstract class BaseApiIntegrationTest
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
            testDatabaseConnection = string.Format(GetConnectionString(), ApiIntegrationTestsStartup.Guid);

            using (var blogDbContext = CreateBlogDbContext())
            {
                blogDbContext.Database.EnsureCreated();
                DataSeeder.Seed(blogDbContext);
            }

            StartServer();
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

        private static TestServer testServer;
        protected static HttpClient Client { get; private set; }

        static void StartServer() {
            var builder = new WebHostBuilder()
                  .UseContentRoot(GetContentRootPath())
                  .UseEnvironment("Development")
                  .UseStartup<ApiIntegrationTestsStartup>();  // Uses Start up class from your API Host project to configure the test server

            testServer = new TestServer(builder);
            Client = testServer.CreateClient();
        }

        private static string GetContentRootPath()
        {
            var testProjectPath = AppDomain.CurrentDomain.BaseDirectory;
            var relativePathToHostProject = @"..\..\..\..\..\DotNetCore\DotNetCore";
            return Path.Combine(testProjectPath, relativePathToHostProject);
        }

        protected static string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
    }
}
