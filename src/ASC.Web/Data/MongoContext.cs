using ASC.DataAccess.Interfaces;
using ASC.Web.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Data
{
    public class MongoContext : IMongoContext
    {
        private IMongoDatabase Database { get; set; }
        public IClientSessionHandle Session { get; set; }
        public MongoClient MongoClient { get; set; }

        private readonly List<Func<Task>> _commands;

        private readonly IConfiguration _configuration;

        public MongoContext(IConfiguration configuration)
        {
            // Configurar o GUID com - (dash) para o CSharp 
            //BsonDefaults.GuidRepresentation = GuidRepresentation.CSharpLegacy;
            RegisterConventions();

            _configuration = configuration ?? throw new ArgumentNullException();

            // Every command will be stored and it'll be processed at SaveChanges
            _commands = new List<Func<Task>>();

            ConfigureMongo();
        }

        public async Task<int> SaveChangesAsync()
        {
            //ConfigureMongo();

            IEnumerable<Task> commandTasks = _commands.Select(command => command());
            await Task.WhenAll(commandTasks);

            int numberTasks = _commands.Count;
            _commands.Clear();

            return numberTasks;

            //using (Session = await MongoClient.StartSessionAsync())
            //{
            //    Session.StartTransaction();

            //    var commandTasks = _commands.Select(c => c());

            //    await Task.WhenAll(commandTasks);

            //    await Session.CommitTransactionAsync();
            //}

            //return _commands.Count;

        }

        private int SaveChanges()
        {
            var qtd = _commands.Count;
            foreach (var command in _commands)
            {
                command();
            }

            _commands.Clear();
            return qtd;
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            // ConfigureMongo();
            return Database.GetCollection<T>(name);
        }

        private void ConfigureMongo()
        {
            if (MongoClient != null)
            {
                return;
            }

            // Configure mongo (You can inject the config, just to simplify)
            MongoClient = new MongoClient(_configuration.GetSection("AppMongoSettings:ConnectionString").Value);
            Database = MongoClient.GetDatabase(_configuration.GetSection("AppMongoSettings:Database").Value);

        }

        private void RegisterConventions()
        {
            var conventionPack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true),
                new IgnoreIfDefaultConvention(true)
            };

            ConventionRegistry.Register("My Solution Conventions", conventionPack, type => true);
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
                Session?.Dispose();
            }

            _isDisposed = true;
        }

        ~MongoContext()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        #endregion


    }
}
