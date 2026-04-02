using Daira.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Daira.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> CommitAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
