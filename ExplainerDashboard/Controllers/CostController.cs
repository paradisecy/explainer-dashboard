using DevExtreme.AspNet.Data;
using ExplainerDashboard.Model;
using ExplainerDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExplainerDashboard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CostController : ControllerBase
    {
        private IMongoService _mongoService;
        public CostController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("get-costs")]
        public object GetCosts(DataSourceLoadOptions loadOptions,int day)
        {
            var collection = _mongoService.Database.GetCollection<FeedCost>("feedcost");
            var data = collection.Find(f=>true)
                .SortByDescending(s => s.DayFrom)
                .ToList();

            return DataSourceLoader.Load(data, loadOptions);
;
        }

        [HttpPost("create-cost")]
        public async Task<IActionResult> Create([FromForm] string values)
        {
            var collection = _mongoService.Database.GetCollection<FeedCost>("feedcost");

            var feed = new FeedCost();

            JsonConvert.PopulateObject(values, feed);

            if (!TryValidateModel(feed))
                return BadRequest();

            await collection.InsertOneAsync(feed);


            return Ok(feed);
        }


        [HttpDelete("delete-cost")]
        public async Task<IActionResult> Delete([FromForm] string key)
        {
            var collection = _mongoService.Database.GetCollection<FeedCost>("feedcost");
            await collection.DeleteOneAsync(
                  Builders<FeedCost>.Filter.Eq(e => e.Id, key.ToString()));

            return Ok(key);
        }


        [HttpPut("update-cost")]
        public async Task<IActionResult> Update([FromForm] string key, [FromForm] string values)
        {

            var collection = _mongoService.Database.GetCollection<FeedCost>("feedcost");

            var filter = Builders<FeedCost>.Filter.Eq(s => s.Id, key.ToString());
            var entry = collection.Find(filter).SingleOrDefault();

            JsonConvert.PopulateObject(values, entry);

            await collection.ReplaceOneAsync(
                  Builders<FeedCost>.Filter.Eq(e => e.Id, key.ToString()), entry);

            return Ok(key);
        }
    }
}
