using DotNetCore.Contracts;
using DotNetCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Services
{
    public abstract class BaseService : IBaseService
    {
        public BaseService(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IDbContext DbContext { get; }

        public virtual void Create<T>(T entity, bool saveChanges = false) where T : BaseEntity
        {
            DbContext.Set<T>().Add(entity);
            SaveChanges(saveChanges);
        }

        public virtual void Create<T>(IList<T> entities, bool saveChanges = false) where T : BaseEntity
        {
            if (entities != null && entities.Count > 0)
            {
                foreach (var entity in entities)
                {
                    DbContext.Set<T>().Add(entity);
                }
                SaveChanges(saveChanges);
            }
        }

        public virtual void Update<T>(T entity, bool saveChanges = false) where T : BaseEntity
        {
            DbContext.Set<T>().Update(entity);
            SaveChanges(saveChanges);
        }

        public virtual void Delete<T>(T entity, bool saveChanges = false) where T : BaseEntity
        {
            DbContext.Set<T>().Remove(entity);
            SaveChanges(saveChanges);
        }

        private void SaveChanges(bool saveChanges)
        {
            if (saveChanges)
                DbContext.SaveChanges();
        }
    }
}