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

        public void Create<TEntity>(IList<TEntity> entities, bool saveChanges = false) where TEntity : BaseEntity
        {
            Create(entities);
            SaveChanges(saveChanges);
        }

        public virtual void Create<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : BaseEntity
        {
            Create<TEntity>(new List<TEntity>() { entity }, saveChanges);
            SaveChanges(saveChanges);
        }

        public virtual async Task<int> CreateAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            Create<TEntity>(new List<TEntity>() { entity });
            return await dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> CreateAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
        {
            Create<TEntity>(entities);
            return await dbContext.SaveChangesAsync();
        }

        public virtual void Update<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : BaseEntity
        {
            Update<TEntity>(new List<TEntity>() { entity });
            SaveChanges(saveChanges);
        }

        public virtual void Update<TEntity>(IList<TEntity> entities, bool saveChanges = false) where TEntity : BaseEntity
        {
            Update<TEntity>(entities);
            SaveChanges(saveChanges);
        }

        public virtual async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            Update<TEntity>(new List<TEntity>() { entity });
            return await dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> UpdateAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
        {
            Update<TEntity>(entities);
            return await dbContext.SaveChangesAsync();
        }

        public virtual void Delete<TEntity>(object id) where TEntity : BaseEntity
        {
            TEntity entity = dbContext.Set<TEntity>().Find(id);
            Delete(entity);
        }

        public virtual async Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            dbContext.Set<TEntity>().Remove(entity);
            return await dbContext.SaveChangesAsync();
        }

        public virtual void Delete<TEntity>(TEntity entity, bool saveChanges = false) where TEntity : BaseEntity
        {
            dbContext.Set<TEntity>().Remove(entity);
            SaveChanges(saveChanges);
        }

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          string includeProperties = null,
          int? skip = null,
          int? take = null)
          where TEntity : BaseEntity
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = dbContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        public virtual IEnumerable<TEntity> GetAll<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : BaseEntity
        {
            return GetQueryable<TEntity>(null, orderBy, includeProperties, skip, take).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : BaseEntity
        {
            return await GetQueryable<TEntity>(null, orderBy, includeProperties, skip, take).ToListAsync();
        }

        public virtual IEnumerable<TEntity> Get<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : BaseEntity
        {
            return GetQueryable<TEntity>(filter, orderBy, includeProperties, skip, take).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null)
            where TEntity : BaseEntity
        {
            return await GetQueryable<TEntity>(filter, orderBy, includeProperties, skip, take).ToListAsync();
        }

        public virtual TEntity GetOne<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
            where TEntity : BaseEntity
        {
            return GetQueryable<TEntity>(filter, null, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null)
            where TEntity : BaseEntity
        {
            return await GetQueryable<TEntity>(filter, null, includeProperties).SingleOrDefaultAsync();
        }

        public virtual TEntity GetFirst<TEntity>(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "")
           where TEntity : BaseEntity
        {
            return GetQueryable<TEntity>(filter, orderBy, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstAsync<TEntity>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
            where TEntity : BaseEntity
        {
            return await GetQueryable<TEntity>(filter, orderBy, includeProperties).FirstOrDefaultAsync();
        }

        public virtual TEntity GetById<TEntity>(object id)
            where TEntity : BaseEntity
        {
            return dbContext.Set<TEntity>().Find(id);
        }

        public virtual Task<TEntity> GetByIdAsync<TEntity>(object id)
            where TEntity : BaseEntity
        {
            return dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual int GetCount<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : BaseEntity
        {
            return GetQueryable<TEntity>(filter).Count();
        }

        public virtual Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : BaseEntity
        {
            return GetQueryable<TEntity>(filter).CountAsync();
        }

        public virtual bool GetExists<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : BaseEntity
        {
            return GetQueryable<TEntity>(filter).Any();
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : BaseEntity
        {
            return GetQueryable<TEntity>(filter).AnyAsync();
        }

        #region Helpers

        private void Create<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
        {
            foreach (var entity in entities)
            {
                if (entity is AuditableEntity)
                {
                    var auditableEntity = entity as AuditableEntity;
                    auditableEntity.CreatedDate = DateTime.UtcNow;
                    // TODO
                    //auditableEntity.CreatedBy = membershipService.CurrentUser.Id;
                }
                dbContext.Set<TEntity>().Add(entity);
            }
        }

        private void Update<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
        {
            foreach (var entity in entities)
            {
                if (entity is AuditableEntity)
                {
                    var auditableEntity = entity as AuditableEntity;
                    auditableEntity.ModifiedDate = DateTime.UtcNow;
                    // TODO
                    //auditableEntity.ModifiedBy = membershipService.CurrentUser.Id;
                }
            }
            dbContext.Set<TEntity>().UpdateRange(entities);
        }

        private void SaveChanges(bool saveChanges)
        {
            if (saveChanges)
                dbContext.SaveChanges(saveChanges);
        }

        #endregion
    }
}
