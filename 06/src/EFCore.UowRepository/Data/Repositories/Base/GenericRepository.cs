using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.UowRepository.Data.Repositories.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationContext context)
        {
            _dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.CountAsync(expression);
        }

        public async Task<T> FirstAsync(Expression<System.Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetDataAsync(Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Expression<Func<T, object>> orderBy = null,
            int? skip = null,
            int? take = null)
        {
            var query = _dbSet.AsQueryable();

            if (expression != null)
                query = query.Where(expression);

            if (include != null)
                query = include(query);

            if (orderBy != null)
                query = query.OrderByDescending(orderBy);

            if (skip != null && skip.HasValue)
                query = query.Skip(skip.Value);

            if (take != null && take.HasValue)
                query = query.Take(take.Value);

            query.OrderBy(x => x);

            return await query.ToListAsync();
        }
    }
}