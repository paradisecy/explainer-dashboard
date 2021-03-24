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
    public class PerformanceController : ControllerBase
    {
        private IMongoService _mongoService;
        public PerformanceController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("get-performance")]
        public object GetPerformance(DataSourceLoadOptions loadOptions,int day)
        {
            var collection = _mongoService.Database.GetCollection<Performance>("performance");
            var data = collection.Find(f=>true)
                .ToList();

            return DataSourceLoader.Load(data, loadOptions);
;
        }

        [HttpPost("create-performance")]
        public async Task<IActionResult> Create([FromForm] string values)
        {
            var collection = _mongoService.Database.GetCollection<Performance>("performance");

            var perf = new Performance();

            JsonConvert.PopulateObject(values, perf);

            if (!TryValidateModel(perf))
                return BadRequest();

            await collection.InsertOneAsync(perf);


            return Ok(perf);
        }


        [HttpDelete("delete-performance")]
        public async Task<IActionResult> Delete([FromForm] string key)
        {
            var collection = _mongoService.Database.GetCollection<Performance>("performance");
            await collection.DeleteOneAsync(
                  Builders<Performance>.Filter.Eq(e => e.Id, key.ToString()));

            return Ok(key);
        }


        [HttpPut("update-performance")]
        public async Task<IActionResult> Update([FromForm] string key, [FromForm] string values)
        {

            var collection = _mongoService.Database.GetCollection<Performance>("performance");

            var filter = Builders<Performance>.Filter.Eq(s => s.Id, key.ToString());
            var entry = collection.Find(filter).SingleOrDefault();

            JsonConvert.PopulateObject(values, entry);

            await collection.ReplaceOneAsync(
                  Builders<Performance>.Filter.Eq(e => e.Id, key.ToString()), entry);

            return Ok(key);
        }
    }
}
