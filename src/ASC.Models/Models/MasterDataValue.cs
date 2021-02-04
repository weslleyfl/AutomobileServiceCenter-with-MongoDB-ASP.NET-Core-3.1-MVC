using ASC.Models.BaseTypes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Models.Models
{
    public class MasterDataValue : BaseEntity, IAuditTracker
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //[BsonIgnoreIfDefault]
        //public ObjectId Id { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        public bool IsActive { get; set; }
        public string Name { get; set; }

        public MasterDataValue() { }
        public MasterDataValue(string masterDataPartitionKey, string value)
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = masterDataPartitionKey;
            CreatedDate = DateTime.Now;
        }

    }
}
