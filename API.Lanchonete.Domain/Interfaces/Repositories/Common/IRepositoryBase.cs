using System;

namespace API.Lanchonete.Domain.Interfaces.Repositories.Common
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : class
    {
        Task SaveChangesAsync();
    }
}
