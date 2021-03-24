using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExplainerDashboard.Model
{
    public class Performance
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Day { get; set; }
        public decimal Weight { get; set; }
        public decimal Feed { get; set; }
        public decimal Fcr { get; set; }
    }
}
