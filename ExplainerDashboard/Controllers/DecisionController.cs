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
    public class DecisionController : ControllerBase
    {
        private IMongoService _mongoService;
        public DecisionController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("get-decisions")]
        public object GetDecision(DataSourceLoadOptions loadOptions,int day)
        {
            var collection = _mongoService.Database.GetCollection<Decision>("decisions");
            var filter = Builders<Decision>.Filter.Eq(s =>s.Day,day);
            var data = collection.Find(filter)
                .SortByDescending(s => s.Day)
                .ToList();

            return DataSourceLoader.Load(data, loadOptions);
;
        }

        [HttpPost("create-decision")]
        public async Task<IActionResult> Create([FromForm] string values)
        {
            var collection = _mongoService.Database.GetCollection<Decision>("decisions");

            var decision = new Decision();

            JsonConvert.PopulateObject(values, decision);

            if (!TryValidateModel(decision))
                return BadRequest();

            await collection.InsertOneAsync(decision);


            return Ok(decision);
        }


        [HttpDelete("delete-decision")]
        public async Task<IActionResult> Delete([FromForm] string key)
        {
            var collection = _mongoService.Database.GetCollection<Decision>("decisions");
            await collection.DeleteOneAsync(
                  Builders<Decision>.Filter.Eq(e => e.Id, key.ToString()));

            return Ok(key);
        }


        [HttpPut("update-decision")]
        public async Task<IActionResult> Update([FromForm] string key, [FromForm] string values)
        {

            var collection = _mongoService.Database.GetCollection<Decision>("decisions");

            var filter = Builders<Decision>.Filter.Eq(s => s.Id, key.ToString());
            var entry = collection.Find(filter).SingleOrDefault();

            JsonConvert.PopulateObject(values, entry);

            await collection.ReplaceOneAsync(
                  Builders<Decision>.Filter.Eq(e => e.Id, key.ToString()), entry);

            return Ok(key);
        }
    }
}
