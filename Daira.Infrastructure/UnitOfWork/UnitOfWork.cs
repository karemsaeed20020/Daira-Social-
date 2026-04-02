using Daira.Application.Interfaces;
using Daira.Infrastructure.Persistence.DbContext;
using Daira.Infrastructure.Repositories.MainRepository;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daira.Infrastructure.UnitOfWork
{
    public class UnitOfWork(DairaDbContext context) : IUnitOfWork
    {
        private readonly Dictionary<string, object> repositories = new();
        public async Task<IDbContextTransaction> BeginTransactionAsync() => await context.Database.BeginTransactionAsync();

        public Task<int> CommitAsync() => context.SaveChangesAsync();

        public void Dispose() => context.Dispose();

        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            var key = typeof(T).Name;
            if (!repositories.ContainsKey(key))
            {
                var repo = new GenericRepository<T>(context);
                repositories.Add(key, repo);
            }
            return (IRepository<T>)repositories[key];

        }
    }
}
