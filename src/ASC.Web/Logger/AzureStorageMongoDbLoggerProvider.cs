using ASC.Business.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Logger
{
    public class AzureStorageMongoDbLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ILogDataOperations _logOperations;

        public AzureStorageMongoDbLoggerProvider(Func<string, LogLevel, bool> filter, ILogDataOperations logOperations)
        {
            _logOperations = logOperations;
            _filter = filter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AzureStorageMongoDbLogger(categoryName, _filter, _logOperations);
        }

        public void Dispose()
        {
            
        }
    }
}
