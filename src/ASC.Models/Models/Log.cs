using ASC.Models.BaseTypes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Models.Models
{
    public class Log : BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string Message { get; set; }

        public Log() { }
        public Log(string key)
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = key;
            CreatedDate = DateTime.Now;
        }


    }
}
