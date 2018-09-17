using DotNetCore.Database.Entities;
using DotNetCore.Database.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace DotNetCore.Database
{
    public class BlogDbContext : BaseDbContext, IBlogDbContext
    {
        public BlogDbContext(DbContextOptions<DbContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleFeedback> ArticleFeedbacks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // TODO 
                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DotNetCore.Constants.Database.SchemaName);

            var applyGenericMethods = typeof(ModelBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var applyGenericApplyConfigurationMethods = applyGenericMethods.Where(m => m.IsGenericMethod && m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));
            var applyGenericMethod = applyGenericApplyConfigurationMethods.Where(m => m.GetParameters().FirstOrDefault().ParameterType.Name == "IEntityTypeConfiguration`1").FirstOrDefault();

            var applicableTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(c => c.IsClass && !c.IsAbstract && !c.ContainsGenericParameters && c.Namespace== "DotNetCore.Database.Mappings");

            foreach (var type in applicableTypes)
            {
                foreach (var @interface in type.GetInterfaces())
                {
                    // if type implements interface IEntityTypeConfiguration<SomeEntity>
                    if (@interface.IsConstructedGenericType && @interface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        // make concrete ApplyConfiguration<SomeEntity> method
                        var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(@interface.GenericTypeArguments[0]);
                        // and invoke that with fresh instance of your configuration type
                        applyConcreteMethod.Invoke(modelBuilder, new object[] { Activator.CreateInstance(type) });
                        break;
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
