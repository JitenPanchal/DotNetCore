using DotNetCore.Contracts;
using DotNetCore.Database;
using DotNetCore.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetCore.Services
{
    public abstract class EntityService : IEntityService
    {
        private readonly IDbContext dbContext;
        private readonly IMembershipService membershipService;

        public EntityService(IDbContext dbContext, IMembershipService membershipService)
        {
            this.dbContext = dbContext;
            this.membershipService = membershipService;
        }

        public virtual async Task<TEntity> GetByIdAsync<TEntity>(int id, bool readOnly = true, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity
        {
            TEntity entity = await GetByIdQuery<TEntity>(id, readOnly).SingleOrDefaultAsync();

            if (throwExceptionOnEntityNotFound && entity == null)
                throw new EntityNotFoundException(id);

            return entity;
        }

        public virtual IQueryable<TEntity> GetByIdQuery<TEntity>(int id, bool readOnly = true) where TEntity : BaseEntity
        {
            var query = dbContext.Set<TEntity>().Where(it => it.Id == id);

            if (readOnly)
                query = query.AsNoTracking();

            return query;
        }

        public virtual TEntity GetById<TEntity>(int id, bool readOnly = true, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity
        {
            TEntity entity = GetByIdQuery<TEntity>(id, readOnly).SingleOrDefault();

            if (throwExceptionOnEntityNotFound && entity == null)
                throw new EntityNotFoundException(typeof(TEntity), id);

            return entity;
        }

        public virtual IQueryable<TEntity> GetByIds<TEntity>(IList<int> ids, bool readOnly = true) where TEntity : BaseEntity
        {
            var query = dbContext.Set<TEntity>().Where(entity => ids.Contains(entity.Id));

            return readOnly ? query.AsNoTracking() : query;
        }

        public virtual IQueryable<TEntity> GetPagedQuery<TEntity>(int pageNumber, int pageSize, bool readOnly = true) where TEntity : BaseEntity
        {
            return GetPagedQuery<TEntity, int>(pageNumber, pageSize, it => it.Id, readOnly);
        }

        public virtual IQueryable<TEntity> GetPagedQuery<TEntity, TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, TKey>> orderBy, bool readOnly = true) where TEntity : BaseEntity
        {
            var query = readOnly ? dbContext.Set<TEntity>().AsNoTracking() : dbContext.Set<TEntity>();

            return query.OrderBy(orderBy).Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public bool IsValidEntityId<TEntity>(int id, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity
        {
            var isValidEntityId = GetByIdQuery<TEntity>(id, true).Count() == 1;

            if (throwExceptionOnEntityNotFound && !isValidEntityId)
                throw new EntityNotFoundException(id);

            return isValidEntityId;
        }

        public void Create<T>(IList<T> entities, bool saveChanges = false) where T : BaseEntity
        {
            foreach (var entity in entities)
            {
                Create<T>(entity, saveChanges);
            }
        }

        public virtual void Create<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : BaseEntity
        {
            if (entity is AuditableEntity)
            {
                var auditableEntity = entity as AuditableEntity;
                auditableEntity.CreatedDate = DateTime.UtcNow;
                // TODO
                //auditableEntity.CreatedBy = membershipService.CurrentUser.Id;
            }
            dbContext.Set<TEntity>().Add(entity);
            SaveChanges(saveChanges);
        }

        public virtual void Update<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : BaseEntity
        {
            if (entity is AuditableEntity)
            {
                var auditableEntity = entity as AuditableEntity;
                auditableEntity.ModifiedDate = DateTime.UtcNow;
                // TODO
                //auditableEntity.ModifiedBy = membershipService.CurrentUser.Id;
            }
            dbContext.Set<TEntity>().Update(entity);
            SaveChanges(saveChanges);
        }

        public virtual void Delete<TEntity>(object id) where TEntity : BaseEntity
        {
            TEntity entity = dbContext.Set<TEntity>().Find(id);
            Delete(entity);
        }

        public virtual void Delete<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : BaseEntity
        {
            dbContext.Set<TEntity>().Remove(entity);
            SaveChanges(saveChanges);
        }

        private void SaveChanges(bool saveChanges)
        {
            if (saveChanges)
                dbContext.SaveChanges(saveChanges);
        }
    }
}
