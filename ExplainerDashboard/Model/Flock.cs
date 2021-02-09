using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExplainerDashboard.Model
{
    public class Flock
    {
        [BsonId]
        [BsonIgnoreIfDefault] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Day { get; set; }
        public int GrowingDay { get; set; }
        public string PlantName { get; set; }
        public string Group { get; set; }
        public decimal CoefficientVariation { get; set; }
        public decimal Fcr { get; set; }
        public decimal Yield { get; set; }
        public decimal MortalityRate { get; set; }
        public int LiveChickQuantity { get; set; }
        public decimal AverageWeight { get; set; }
        public int Distance { get; set; }
        public decimal ClassAPercentage { get; set; }
        public decimal ClassBPercentage { get; set; }


    }
}
