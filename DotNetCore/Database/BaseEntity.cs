using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Database
{
    public abstract class BaseEntity
    {
        protected static readonly IEnumerable<ValidationResult> EmptyValidationErrors = Enumerable.Empty<ValidationResult>();

        public int Id { get; set; }

        internal virtual void OnBeforeSaveEntity(EntityEntry entityEntry) { }

        protected static IDbContext GetDbContext(IDictionary<object, object> items)
        {
            return GetDictionaryItem<IDbContext>(items, Constants.Database.DbContext);
        }

        protected static EntityEntry GetEntityEntry(IDictionary<object, object> items)
        {
            return GetDictionaryItem<EntityEntry>(items, Constants.Database.EntityEntry);
        }

        protected static T GetDictionaryItem<T>(IDictionary<object, object> items, string key)
        {
            if (items == null || items.Count == 0 || !items.ContainsKey(key))
                throw new KeyNotFoundException(nameof(items));

            var entry = items[key];

            if (!(entry is T))
                throw new ArgumentException(nameof(items));

            return (T)entry;
        }
    }
}
