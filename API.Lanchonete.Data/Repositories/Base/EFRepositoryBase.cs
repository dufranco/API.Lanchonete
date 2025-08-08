using API.Lanchonete.Data.Context;
using API.Lanchonete.Domain.Interfaces.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Lanchonete.Data.Repositories.Base
{
    public abstract class EFRepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _context;
        private bool _disposed = false;

        protected EFRepositoryBase(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        ~EFRepositoryBase() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _context?.Dispose();

                _disposed = true;
            }
        }

        public virtual async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await _context.Set<TEntity>().AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
            return entity;
        }

        public virtual async Task RemoveAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.Set<TEntity>().Remove(entity);
            await SaveChangesAsync();
        }
    }
}
