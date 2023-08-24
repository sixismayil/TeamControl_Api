using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TeamControlV2.Infrastructure.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> AllQuery { get; }

        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

        void Insert(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        void Save();
    }
}
