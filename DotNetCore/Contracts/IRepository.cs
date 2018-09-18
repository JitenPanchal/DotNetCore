using DotNetCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Contracts
{
    public interface IRepository : IReadOnlyRepository
    {
        void Create<TEntity>(TEntity entity)
           where TEntity : BaseEntity;

        void Update<TEntity>(TEntity entity)
            where TEntity : BaseEntity;

        void Delete<TEntity>(object id)
            where TEntity : BaseEntity;

        void Delete<TEntity>(TEntity entity)
            where TEntity : BaseEntity;

        void Save();

        Task SaveAsync();
    }
}