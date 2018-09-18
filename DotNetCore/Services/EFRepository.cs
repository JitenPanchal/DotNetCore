using DotNetCore.Contracts;
using DotNetCore.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Services
{
    public class EFRepository<TContext> : EFReadOnlyRepository<TContext>, IRepository where TContext : DbContext
    {
        public EFRepository(TContext context) : base(context) {}

        public virtual void Create<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            if (entity is AuditableEntity)
            {
                var auditableEntity = entity as AuditableEntity;
                auditableEntity.CreatedDate = DateTime.UtcNow;
                // TODO
                //auditableEntity.CreatedBy = createdBy;
            }
            dbContext.Set<TEntity>().Add(entity);
        }

        public virtual void Update<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            if (entity is AuditableEntity)
            {
                var auditableEntity = entity as AuditableEntity;
                auditableEntity.ModifiedDate = DateTime.UtcNow;
                // TODO
                //auditableEntity.ModifiedBy = createdBy;
            }
            dbContext.Set<TEntity>().Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete<TEntity>(object id) where TEntity : BaseEntity
        {
            TEntity entity = dbContext.Set<TEntity>().Find(id);
            Delete(entity);
        }

        public virtual void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var dbSet = dbContext.Set<TEntity>();
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public virtual void Save()
        {
            dbContext.SaveChanges();
            // TODO
            //try
            //{
            //    context.SaveChanges();
            //}
            //catch (DbEntityValidationException e)
            //{
            //    ThrowEnhancedValidationException(e);
            //}
        }

        public virtual Task SaveAsync()
        {
            return dbContext.SaveChangesAsync();
            // TODO
            //try
            //{
            //    return context.SaveChangesAsync();
            //}
            //catch (DbEntityValidationException e)
            //{
            //    ThrowEnhancedValidationException(e);
            //}

            //return Task.FromResult(0);
        }

        // TODO
        //protected virtual void ThrowEnhancedValidationException(DbEntityValidationException e)
        //{
        //    var errorMessages = e.EntityValidationErrors
        //            .SelectMany(x => x.ValidationErrors)
        //            .Select(x => x.ErrorMessage);

        //    var fullErrorMessage = string.Join("; ", errorMessages);
        //    var exceptionMessage = string.Concat(e.Message, " The validation errors are: ", fullErrorMessage);
        //    throw new DbEntityValidationException(exceptionMessage, e.EntityValidationErrors);
        //}
    }
}
