using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace DotNetCore
{
    public class TestStartup : Startup
    {
        public readonly static string Guid = System.Guid.NewGuid().ToString("N");

        public TestStartup(IHostingEnvironment env) : base(env)
        {
            configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
        }

        protected override string GetConnectionString()
        {
            return string.Format(configuration.GetConnectionString("connectionString"), Guid);
        }
    }
}
