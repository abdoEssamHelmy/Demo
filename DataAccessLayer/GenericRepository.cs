using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class GenericRepository<TEntity> where TEntity : class, new()
    {
        protected readonly DbContext dbContext;
        public GenericRepository(DbContext DbContext)
        {
            dbContext = DbContext;
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return (await dbContext.Set<TEntity>().AddAsync(entity)).Entity;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbContext.Set<TEntity>().Where(predicate).CountAsync();
        }

        public async Task<int> CountAsync()
        {
            return await dbContext.Set<TEntity>().CountAsync();
        }

        public async Task<TEntity> FindAsync(params object[] predicate)
        {

            var key = dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties.Single();           
            var param = Expression.Parameter(typeof(TEntity), "p");
            var expr = Expression.Lambda<Func<TEntity, bool>>(
                   Expression.Equal(
                       Expression.Property(param, key.Name),

                       ExpressionClosureFactory.GetField(Convert.ChangeType(predicate[0], key.ClrType), key.ClrType)
                   ),
                   param
               );

            return await dbContext.Set<TEntity>().IgnoreQueryFilters().FirstOrDefaultAsync(expr);
        }

        public TEntity Update(TEntity entity)
        {
            return dbContext.Set<TEntity>().Update(entity).Entity;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return dbContext.Set<TEntity>().Where(predicate);
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return dbContext.Set<TEntity>().Where(predicate);
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbContext.Set<TEntity>().AnyAsync(predicate);
        }

        public TEntity Remove(TEntity entity)
        {
            return dbContext.Set<TEntity>().Remove(entity).Entity;
        }

        public List<TEntity> RemoveRange(List<TEntity> list)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = dbContext.Set<TEntity>().Remove(list[i]).Entity;
            return list;
        }

        public async Task<TEntity> FirstOrDefaultAsync()
        {
            return await dbContext.Set<TEntity>().FirstOrDefaultAsync();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbContext.Set<TEntity>().FirstAsync(predicate);
        }
    }
}
