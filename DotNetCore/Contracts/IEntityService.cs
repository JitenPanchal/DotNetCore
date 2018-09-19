﻿using DotNetCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DotNetCore.Contracts
{
    public interface IEntityService
    {
        Task<TEntity> GetByIdAsync<TEntity>(int id, bool readOnly = true, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity;

        IQueryable<TEntity> GetByIdQuery<TEntity>(int id, bool readOnly = true) where TEntity : BaseEntity;

        TEntity GetById<TEntity>(int id, bool readOnly = true, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity;

        IQueryable<TEntity> GetByIds<TEntity>(IList<int> ids, bool readOnly = true) where TEntity : BaseEntity;

        IQueryable<TEntity> GetPagedQuery<TEntity>(int pageNumber, int pageSize, bool readOnly = true) where TEntity : BaseEntity;

        IQueryable<TEntity> GetPagedQuery<TEntity, TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, TKey>> orderBy, bool readOnly = true) where TEntity : BaseEntity;

        bool IsValidEntityId<TEntity>(int id, bool throwExceptionOnEntityNotFound = false) where TEntity : BaseEntity;

        void Create<T>(T entity, bool saveChanges = false) where T : BaseEntity;

        void Create<T>(IList<T> entities, bool saveChanges = false) where T : BaseEntity;

        void Update<T>(T entity, bool saveChanges = false) where T : BaseEntity;

        void Delete<T>(T entity, bool saveChanges = false) where T : BaseEntity;
    }
}
