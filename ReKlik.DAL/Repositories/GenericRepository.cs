using Microsoft.EntityFrameworkCore;
using ReKlik.DAL.DBContext;
using ReKlik.DAL.Repositories.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReKlik.DAL.Repositories
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        protected readonly ReKlikDbContext _context;
        private readonly DbSet<TModel> _dbSet;

        public GenericRepository(ReKlikDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TModel>();
        }

        // ---- Métodos originales ----
        public async Task<TModel> Get(Expression<Func<TModel, bool>> filter)
        {
            return await _dbSet.FirstOrDefaultAsync(filter);
        }

        public async Task<TModel> Create(TModel model)
        {
            await _dbSet.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<bool> Update(TModel model)
        {
            _dbSet.Update(model);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(TModel model)
        {
            _dbSet.Remove(model);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<IQueryable<TModel>> GetAll(Expression<Func<TModel, bool>>? filter = null)
        {
            IQueryable<TModel> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return Task.FromResult(query);
        }

        // ---- Métodos nuevos para que compile con IGenericRepository ----
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TModel> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(TModel model)
        {
            await _dbSet.AddAsync(model);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }
}
