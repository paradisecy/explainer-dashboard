using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExplainerDashboard.Model
{
    public class SuggestedPlant
    {
        public string Plant { get; set; }
        public string Group { get; set; }
    }

    public class Solution
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Kb { get; set; }
        public string Coach { get; set; }
        public string RawInference { get; set; }
        public string Suggest { get; set; }
        public string Explain { get; set; }

        public string SuggestedPlants { get; set; }

        public List<string> SuggestedPlantsList { get; set; }
    }
}
