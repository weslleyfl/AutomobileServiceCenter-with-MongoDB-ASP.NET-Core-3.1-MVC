using ASC.Business.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Logger
{
    /// <summary>
    /// ILogger of ASP.NET Core
    /// </summary>
    public class AzureStorageMongoDbLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ILogDataOperations _logDataOperations;

        public AzureStorageMongoDbLogger(string categoryName, Func<string,
            LogLevel, bool> filter,
            ILogDataOperations logOperations)
        {
            _categoryName = categoryName;
            _filter = filter;
            _logDataOperations = logOperations;
        }
        /// <summary>
        /// The default log levels and order of priority provided by ASP.NET Core are 
        /// Trace = 0, Debug = 1, Information = 2, Warning = 3, Error = 4, Critical = 5, and None = 6.
        /// </summary>       
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel) == false)
            {
                return;
            }

            if (exception == null)
            {
                _logDataOperations.CreateLogAsync(logLevel.ToString(), formatter(state, exception));
            }
            else
            {
                _logDataOperations.CreateExceptionLogAsync(eventId.Name, exception.Message, exception.StackTrace);
            }

        }

        /// <summary>
        /// This method to check whether logging is enabled for a given log level and message 
        /// category (which is in context of execution) by calling the IsEnabled method.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns>true/false</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }


        /// <summary>
        /// The BeginScope method is used to group some log operations based on scope. We can attach the same
        /// data to each log that is created as part of the group
        /// </summary>      
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
