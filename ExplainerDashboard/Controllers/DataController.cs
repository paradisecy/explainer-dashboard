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
    public class DataController : ControllerBase
    {
        private IMongoService _mongoService;
        public DataController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("get-flocks")]
        public object GetFlocks(DataSourceLoadOptions loadOptions,int day)
        {
            var collection = _mongoService.Database.GetCollection<Flock>("flocks");
            var filter = Builders<Flock>.Filter.Eq(s => s.Day, day);
            var data = collection.Find(filter)
                .SortByDescending(s => s.Day)
                .ToList();

            return DataSourceLoader.Load(data, loadOptions);
;
        }

        [HttpPost("create-flock")]
        public async Task<IActionResult> Create([FromForm] string values)
        {
            var collection = _mongoService.Database.GetCollection<Flock>("flocks");

            var flock = new Flock();

            JsonConvert.PopulateObject(values, flock);

            if (!TryValidateModel(flock))
                return BadRequest();

            await collection.InsertOneAsync(flock);


            return Ok(flock);
        }


        [HttpDelete("delete-flock")]
        public async Task<IActionResult> Delete([FromForm] string key)
        {
            var collection = _mongoService.Database.GetCollection<Flock>("flocks");
            await collection.DeleteOneAsync(
                  Builders<Flock>.Filter.Eq(e => e.Id, key.ToString()));

            return Ok(key);
        }


        [HttpPut("update-flock")]
        public async Task<IActionResult> Update([FromForm] string key, [FromForm] string values)
        {

            var collection = _mongoService.Database.GetCollection<Flock>("flocks");

            var filter = Builders<Flock>.Filter.Eq(s => s.Id, key.ToString());
            var entry = collection.Find(filter).SingleOrDefault();

            JsonConvert.PopulateObject(values, entry);

            await collection.ReplaceOneAsync(
                  Builders<Flock>.Filter.Eq(e => e.Id, key.ToString()), entry);

            return Ok(key);
        }
    }
}
