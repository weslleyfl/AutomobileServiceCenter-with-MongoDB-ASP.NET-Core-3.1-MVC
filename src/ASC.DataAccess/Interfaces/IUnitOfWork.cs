using ASC.Models.BaseTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //Queue<Task<Action>> RollbackActions { get; set; }
        //string ConnectionString { get; set; }
        //void CommitTransaction();
        IRepository<T> Repository<T>() where T : BaseEntity;
        Task<bool> CommitTransactionAsync();

    }
}
