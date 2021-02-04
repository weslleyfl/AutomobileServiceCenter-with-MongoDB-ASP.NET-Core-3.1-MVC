using ASC.Models.BaseTypes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Models.Models
{
    public class ExceptionLog : BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string Message { get; set; }
        public string Stacktrace { get; set; }

        public ExceptionLog() { }
        public ExceptionLog(string key)
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = DateTime.UtcNow.ToString();
            CreatedDate = DateTime.Now;
        }
     
    }
}
