using ASC.Models.BaseTypes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Models.Models
{
    /// <summary>
    /// Solicitação do serviço a ser prestado pelos engenheiros.
    /// </summary>
    public class ServiceRequest : BaseEntity, IAuditTracker
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string VehicleName { get; set; }
        public string VehicleType { get; set; }
        public string Status { get; set; }
        public string RequestedServices { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ServiceEngineer { get; set; }


        public ServiceRequest() { }

        public ServiceRequest(string email)
        {
            base.RowKey = Guid.NewGuid().ToString();
            base.PartitionKey = email;
        }
    }
}
