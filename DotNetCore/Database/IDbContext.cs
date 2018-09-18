using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCore.Database
{
    public interface IDbContext
    {
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<TEntity> GetByIdAsync<TEntity>(int id, bool readOnly = true, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity;

        IQueryable<TEntity> GetByIdQuery<TEntity>(int id, bool readOnly = true) where TEntity : BaseEntity;

        TEntity GetById<TEntity>(int id, bool readOnly = true, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity;

        IQueryable<TEntity> GetByIds<TEntity>(IList<int> ids, bool readOnly = true) where TEntity : BaseEntity;

        IQueryable<TEntity> GetPagedQuery<TEntity>(int pageNumber, int pageSize, bool readOnly = true) where TEntity : BaseEntity;

        IQueryable<TEntity> GetPagedQuery<TEntity, TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, TKey>> orderBy, bool readOnly = true) where TEntity : BaseEntity;

        bool IsValidEntityId<TEntity>(int id, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity;

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        int SaveChanges(bool acceptAllChangesOnSuccess);

        int SaveChanges();

    }
}