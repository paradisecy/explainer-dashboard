using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExplainerDashboard.Model
{
    public class Demand
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Day { get; set; }
        public int Quantity { get; set; }
        public string Class { get; set; }
        public decimal FromWeight{ get; set; }
        public decimal ToWeight { get; set; }
    }
}
