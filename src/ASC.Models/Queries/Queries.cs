using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Models.Queries
{
    public class Queries
    {
        public static FilterDefinition<T> GetDashboardQuery<T>(DateTime? requestedDate,
            List<string> status = null,
            string email = "",
            string serviceEngineerEmail = "")
        {
            FilterDefinition<T> finalQuery = null;
            var builder = Builders<T>.Filter;
            var statusQueries = new List<string>();

            // Add Requested Date Clause
            if (requestedDate.HasValue)
            {
                finalQuery = builder.Gte("RequestedDate", requestedDate.Value);
            }

            // Add Email clause if email is passed as a parameter
            if (string.IsNullOrWhiteSpace(email) == false)
            {
                finalQuery = builder.Eq("email", email);
            }

            return finalQuery;

        }
    }
}
