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
    }
}
