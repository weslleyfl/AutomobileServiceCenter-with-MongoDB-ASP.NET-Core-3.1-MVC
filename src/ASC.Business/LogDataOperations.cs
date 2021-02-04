using ASC.Business.Interfaces;
using ASC.DataAccess.Interfaces;
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business
{
    public class LogDataOperations : ILogDataOperations
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogDataOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateExceptionLogAsync(string id, string message, string stacktrace)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Repository<ExceptionLog>().Add(new ExceptionLog()
                {
                    RowKey = id,
                    PartitionKey = "Exception",
                    Message = message,
                    Stacktrace = stacktrace,
                    CreatedDate = DateTime.Now
                });

                await _unitOfWork.CommitTransactionAsync();
            }
        }

        public async Task CreateLogAsync(string category, string message)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Repository<Log>().Add(new Log()
                {
                    RowKey = Guid.NewGuid().ToString(),
                    PartitionKey = category,
                    Message = message,
                    CreatedDate = DateTime.Now

                });

                await _unitOfWork.CommitTransactionAsync();
            }
        }

        public async Task CreateUserActivityAsync(string email, string action)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Repository<UserActivity>().Add(new UserActivity()
                {
                    RowKey = Guid.NewGuid().ToString(),
                    PartitionKey = email,
                    Action = action,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                });

                await _unitOfWork.CommitTransactionAsync();
            }
        }
    }
}
