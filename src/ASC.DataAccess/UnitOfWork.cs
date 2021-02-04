using ASC.DataAccess.Interfaces;
using ASC.Models.BaseTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoContext _context;
        private Dictionary<string, object> _repositories;

        public UnitOfWork(IMongoContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitTransactionAsync()
        {
            var changeAmount = await _context.SaveChangesAsync();
            return changeAmount > 0;
        }

        public IRepository<T> Repository<T>() where T : BaseEntity
        {

            if (_repositories == null)
            {
                _repositories = new Dictionary<string, object>();
            }

            var type = typeof(T).Name;

            if (_repositories.ContainsKey(type))
                return (IRepository<T>)_repositories[type];

            var repositoryType = typeof(Repository<>);

            var repositoryInstance = Activator
                .CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);

            _repositories.Add(type, repositoryInstance);

            return (IRepository<T>)_repositories[type];

        }


        #region " Dispose "

        private bool _isDisposed;

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                // free managed resources
                _context?.Dispose();
                _repositories = null;
            }

            _isDisposed = true;
        }

        ~UnitOfWork()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        #endregion
    }
}
