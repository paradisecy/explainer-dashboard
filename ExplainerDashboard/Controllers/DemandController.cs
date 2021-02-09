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
    public class DemandController : ControllerBase
    {
        private IMongoService _mongoService;
        public DemandController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("get-demand")]
        public object GetDemand(DataSourceLoadOptions loadOptions,int day)
        {
            var collection = _mongoService.Database.GetCollection<Demand>("demands");
            var filter = Builders<Demand>.Filter.Eq(s =>s.Day,day);
            var data = collection.Find(filter)
                .SortByDescending(s => s.Day)
                .ToList();

            return DataSourceLoader.Load(data, loadOptions);
;
        }

        [HttpPost("create-demand")]
        public async Task<IActionResult> Create([FromForm] string values)
        {
            var collection = _mongoService.Database.GetCollection<Demand>("demands");

            var demand = new Demand();

            JsonConvert.PopulateObject(values, demand);

            if (!TryValidateModel(demand))
                return BadRequest();

            await collection.InsertOneAsync(demand);


            return Ok(demand);
        }


        [HttpDelete("delete-demand")]
        public async Task<IActionResult> Delete([FromForm] string key)
        {
            var collection = _mongoService.Database.GetCollection<Demand>("demands");
            await collection.DeleteOneAsync(
                  Builders<Demand>.Filter.Eq(e => e.Id, key.ToString()));

            return Ok(key);
        }


        [HttpPut("update-demand")]
        public async Task<IActionResult> Update([FromForm] string key, [FromForm] string values)
        {

            var collection = _mongoService.Database.GetCollection<Demand>("demands");

            var filter = Builders<Demand>.Filter.Eq(s => s.Id, key.ToString());
            var entry = collection.Find(filter).SingleOrDefault();

            JsonConvert.PopulateObject(values, entry);

            await collection.ReplaceOneAsync(
                  Builders<Demand>.Filter.Eq(e => e.Id, key.ToString()), entry);

            return Ok(key);
        }
    }
}
