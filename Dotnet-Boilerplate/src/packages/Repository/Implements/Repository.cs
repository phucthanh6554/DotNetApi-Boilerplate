using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public abstract class Repository<T> where T : class
    {
        private readonly DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> FindAll()
        {
            return _dbContext.Set<T>();
        }

        public EntityEntry<T> Add(T model)
        {
            return _dbContext.Set<T>().Add(model);
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> func)
        {
            return _dbContext.Set<T>().Where(func);
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default) {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
