using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Database.Entities
{
    public partial class Article : IValidatableObject
    {
        private readonly IBlogDbContext blogDbContext;

        public Article(IBlogDbContext blogDbContext)
        {
            this.blogDbContext = blogDbContext;
        }

        public Article()
        {
        }

        const string TitleError = "Article with title '{0}' already exists.";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbEntityEntry = GetEntityEntry(validationContext.Items);

            if (dbEntityEntry.State == EntityState.Deleted)
                return EmptyValidationErrors;

            var dbContext = GetDbContext(validationContext.Items);

            if (dbEntityEntry.State == EntityState.Added || dbEntityEntry.State == EntityState.Modified)
                return ValidateOnAddOrUpdate(dbEntityEntry, dbContext);
            else
                return EmptyValidationErrors;
        }

        private IEnumerable<ValidationResult> ValidateOnAddOrUpdate(EntityEntry entityEntry, IDbContext dbContext)
        {
            var entityCount = dbContext.Set<Article>().AsNoTracking().Where(it => it.Title == Title && Id != it.Id).Count();

            IList<ValidationResult> validationResults = new List<ValidationResult>();

            if (entityCount > 0)
            {
                validationResults.Add(new ValidationResult(string.Format(TitleError, this.Title), new string[] { nameof(Title) }));
            }

            return validationResults;
        }

    }
}
