using ASC.DataAccess.Interfaces;
using ASC.Models.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.DataAccess.Repository
{
    public class ServiceRequestRepository : Repository<ServiceRequest>, IServiceRequestRepository
    {
        public ServiceRequestRepository(IMongoContext context) : base(context)
        {
        }

        /// <summary>
        /// Filter multiple parameters with MongoDB provider
        /// https://stackoverflow.com/questions/43297901/filter-multiple-parameters-with-mongodb-provider
        /// https://zetcode.com/csharp/mongodb/
        /// </summary>       
        public async Task<IEnumerable<ServiceRequest>> GetDashboardQueryAsync(DateTime? requestedDate,
            List<string> status = null,
            string email = "",
            string serviceEngineerEmail = "")
        {
            var builder = Builders<ServiceRequest>.Filter;
            IList<FilterDefinition<ServiceRequest>> filters = new List<FilterDefinition<ServiceRequest>>();

            // Add Requested Date Clause
            if (requestedDate.HasValue)
            {
                filters.Add(builder.Gte("RequestedDate", requestedDate.Value));
            }

            // Add Email clause if email is passed as a parameter
            if (!string.IsNullOrWhiteSpace(email))
            {
                filters.Add(builder.Eq("PartitionKey", email));
            }

            // Add Service Engineer Email clause if email is passed as a parameter
            if (!string.IsNullOrWhiteSpace(serviceEngineerEmail))
            {
                filters.Add(builder.Eq("ServiceEngineer", serviceEngineerEmail));
            }

            // Add Status clause if status is passed a parameter.
            // Individual status clauses are appended with OR Condition
            if (status != null)
            {
                filters.Add(builder.In(s => s.Status, status));
            }

            //if (status != null)
            //{
            //    foreach (var state in status)
            //    {
            //        filters.Add(builder.In("Status", state));
            //    }
            //}


            var filterConcat = builder.Or(filters);

            // Here's how you can debug the generated query
            //var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<ServiceRequest>();
            //var renderedFilter = filterConcat.Render(documentSerializer, BsonSerializer.SerializerRegistry).ToString();

            IAsyncCursor<ServiceRequest> results = await DbSet.FindAsync(filterConcat);

            return results.ToList();
        }

        /// <summary>
        /// Return status services
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<ServiceRequest>> GetStatusServiceRequestsAsync(List<string> status)
        {

            IAsyncCursor<ServiceRequest> results = await DbSet.FindAsync(Builders<ServiceRequest>.Filter.In("Status", status));
            return results.ToList();
        }
    }
}
