using ASC.DataAccess.Interfaces;
using ASC.Models.BaseTypes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using MongoDB.Bson;

namespace ASC.DataAccess
{
    /// <summary>
    /// https://github.com/brunohbrito/MongoDB-RepositoryUoWPatterns/tree/ccc8537e21a6d1491a99d290a530b0d8ac01712c/MongoDB.GenericRepository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : BaseEntity, new()
    {
        protected readonly IMongoContext Context;
        protected IMongoCollection<T> DbSet;

        public Repository(IMongoContext context)
        {
            Context = context;
            // Table e a collection no mongodb
            // DbSet = Context.GetCollection<T>(typeof(T).Name);
            ConfigDbSet();
        }

        public virtual void Add(T obj)
        {
            Context.AddCommand(() => DbSet.InsertOneAsync(obj));
        }

        public virtual void Remove(Guid id)
        {
            Context.AddCommand(() => DbSet.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id)));
        }

        public virtual void Update(T obj)
        {
            Context.AddCommand(() => DbSet.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", obj.GetId()), obj));
        }

        public virtual async Task<IEnumerable<T>> FindAllAsync()
        {
            IAsyncCursor<T> results = await DbSet.FindAsync(Builders<T>.Filter.Empty);
            return results.ToList();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            IAsyncCursor<T> data = await DbSet.FindAsync(Builders<T>.Filter.Eq("_id", id));
            return data.SingleOrDefault();
        }

        public virtual async Task<IEnumerable<T>> FindAllByPartitionKeyAsync(string partitionkey)
        {

            IAsyncCursor<T> results = await DbSet.FindAsync(Builders<T>.Filter.Eq("PartitionKey", partitionkey));
            // IAsyncCursor<T> data = await DbSet.FindAsync<T>(p => p.PartitionKey == partitionkey);
            return results.ToList();

            //TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionkey));
            //TableContinuationToken tableContinuationToken = null;
            //var result = await storageTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);
            //return result.Results as IEnumerable<T>;
        }

        public virtual async Task<IEnumerable<T>> FindAllByRowKeyAsync(string rowKey)
        {
            IAsyncCursor<T> results = await DbSet.FindAsync(Builders<T>.Filter.Eq("RowKey", rowKey));
            return results.ToList();
        }

        public virtual async Task<T> FindAsync(string rowKey)
        {
            IAsyncCursor<T> results = await DbSet.FindAsync(Builders<T>.Filter.Eq("RowKey", rowKey));
            return results.SingleOrDefault();

            //var docId = new ObjectId(id);
            //return _books.Find<Book>(b => b.Id == docId).FirstOrDefault();

            //TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            //var result = await storageTable.ExecuteAsync(retrieveOperation);
            //return result.Result as T;
        }

        private void ConfigDbSet()
        {
            DbSet = Context.GetCollection<T>(typeof(T).Name);
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
                Context?.Dispose();
            }

            _isDisposed = true;
        }


        ~Repository()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        #endregion
    }
}