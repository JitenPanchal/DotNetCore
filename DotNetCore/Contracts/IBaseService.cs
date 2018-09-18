using DotNetCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Contracts
{
    public interface IBaseService
    {
        IDbContext DbContext { get; }

        void Create<T>(T entity, bool saveChanges = false) where T : BaseEntity;

        void Create<T>(IList<T> entities, bool saveChanges = false) where T : BaseEntity;

        void Update<T>(T entity, bool saveChanges = false) where T : BaseEntity;

        void Delete<T>(T entity, bool saveChanges = false) where T : BaseEntity;
    }
}
