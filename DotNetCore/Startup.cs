using AutoMapper;
using DotNetCore.Contracts;
using DotNetCore.Database;
using DotNetCore.Infrastructure.Filters;
using DotNetCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetCore
{
    public class Startup
    {
        protected IConfigurationRoot configuration;

        private static readonly ILoggerFactory loggerFactory = new LoggerFactory()
          .AddDebug((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name))
          .AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name));

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var env = serviceProvider.GetService<IHostingEnvironment>();

            var mvcOptions = services.AddMvc();

            mvcOptions.AddJsonOptions((serializerSettings) =>
            {
                if (serializerSettings.SerializerSettings.ContractResolver != null)
                {
                    if (!env.IsDevelopment())
                    {
                        serializerSettings.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                    }
                }
            });

            mvcOptions.AddMvcOptions((formatters) =>
            {
                formatters.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                formatters.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(formatters));
            });

            services.AddMvc((options) =>
            {
                // TODO - Authorization ??
                //options.Filters.Add(typeof(ResourceFilter));
                options.Filters.Add(typeof(RequestModelValidationFilter));
                options.Filters.Add(typeof(ExceptionHandlerFilter));
                
            });

            services.AddAutoMapper();
            
            services.AddSingleton(GetDbContextOptionsBuilder(env).Options);

            services.AddDbContext<IBlogDbContext, BlogDbContext>(contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);

            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IMembershipService, MembershipService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }

        #region Helpers

        protected virtual string GetConnectionString()
        {
            return configuration.GetConnectionString("connectionString");
        }

        private DbContextOptionsBuilder<DbContext> GetDbContextOptionsBuilder(IHostingEnvironment env)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DbContext>();

            dbContextOptionsBuilder.UseSqlServer(GetConnectionString());

            if (env.IsDevelopment())
            {
                dbContextOptionsBuilder.UseLoggerFactory(loggerFactory);
            }

            return dbContextOptionsBuilder;
        }

        #endregion
    }
}