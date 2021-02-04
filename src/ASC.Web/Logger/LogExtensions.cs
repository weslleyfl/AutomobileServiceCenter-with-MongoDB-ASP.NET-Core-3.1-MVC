using ASC.Business.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Logger
{
    public static class LogExtensions
    {
        /// <summary>
        /// To add the created AzureStorageLoggerProvider, we will call the AddAzureTableStorageLog method
        /// in the Configure method of the Startup class,
        /// </summary>       
        public static ILoggerFactory AddAzureTableStorageMongoDbLog(this ILoggerFactory factory,
           ILogDataOperations logOperations,
           Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new AzureStorageMongoDbLoggerProvider(filter, logOperations));
            return factory;
        }
    }
}
