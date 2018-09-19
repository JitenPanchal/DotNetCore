using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCore.Database
{
    public abstract class BaseDbContext : DbContext, IDbContext, IDisposable
    {
        public BaseDbContext(DbContextOptions<DbContext> options) : base(options) {}

        public override int SaveChanges()
        {
            ValidateEntities();
            OnBeforeSaveChanges();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ValidateEntities();
            OnBeforeSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateEntities();
            OnBeforeSaveChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateEntities();
            OnBeforeSaveChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ValidateEntities()
        {
            var entityEntries = from entity in ChangeTracker.Entries()
                           where (entity.State == EntityState.Added || entity.State == EntityState.Modified || entity.State == EntityState.Deleted) &&
                           entity.Entity is IValidatableObject
                           select entity;

            var items = new Dictionary<object, object>() { 
                { Constants.Database.EntityEntry, null },
                { Constants.Database.DbContext, this }
            };

            foreach (var entityEntry in entityEntries)
            {
                items[Constants.Database.EntityEntry] = entityEntry;
                var validationContext = new ValidationContext(entityEntry.Entity, items);
                Validator.ValidateObject(entityEntry.Entity, validationContext, validateAllProperties: true);
            }
        }

        private void OnBeforeSaveChanges()
        {
            foreach (var entityEntry in ChangeTracker.Entries())
            {
                (entityEntry.Entity as BaseEntity).OnBeforeSaveEntity(entityEntry);
            }
        }
    }
}
