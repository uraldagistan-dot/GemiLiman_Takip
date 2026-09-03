using Microsoft.EntityFrameworkCore;
using PortTrackingSystem.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System;

namespace PortTrackingSystem.Core.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync(); // EKSİK OLAN SİHİRLİ SATIR BU
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges(); // Kaydetmeyi buraya da ekliyoruz
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges(); // Kaydetmeyi buraya da ekliyoruz
        }
         public async Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Dışarıdan istenen her bir ilişkili tabloyu (Yük, Ziyaret vs.) sorguya dahil ediyoruz (Include)
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithNestedIncludeAsync(params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }
    }
     
}
