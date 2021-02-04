using ASC.Models.BaseTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.DataAccess.Interfaces
{
    public interface IRepository<T>: IDisposable where T : BaseEntity
    {
        //Task<T> AddAsync(T entity);
        //Task<T> UpdateAsync(T entity);
        //Task DeleteAsync(T entity);
        //Task<T> FindAsync(string rowKey);
        //Task<IEnumerable<T>> FindAllByPartitionKeyAsync(string partitionkey);
        //Task<IEnumerable<T>> FindAllAsync();
        //Task CreateTableAsync();

        void Add(T obj);
        void Update(T obj);
        void Remove(Guid id);
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> FindAllAsync();
        Task<IEnumerable<T>> FindAllByPartitionKeyAsync(string partitionkey);
        Task<T> FindAsync(string rowKey);
        Task<IEnumerable<T>> FindAllByRowKeyAsync(string rowKey);

    }
}
