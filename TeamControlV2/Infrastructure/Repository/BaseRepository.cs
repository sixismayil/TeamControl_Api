using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TeamControlV2.Infrastructure.Repository
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _entity;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _entity = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> AllQuery 
        {
            get { return _entity; }
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _entity.Where(predicate);
        }

        public void Insert(TEntity entity)
        {
            _entity.Add(entity);
        }

        public void Remove(TEntity entity)
        {
            _entity.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _entity.Update(entity);
        }
    }
}
