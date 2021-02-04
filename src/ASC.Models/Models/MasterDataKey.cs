using ASC.Models.BaseTypes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Models.Models
{
    public class MasterDataKey : BaseEntity, IAuditTracker
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

        public MasterDataKey() { }

        public MasterDataKey(string key)
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = key;
            CreatedDate = DateTime.Now;
        }



    }
}
