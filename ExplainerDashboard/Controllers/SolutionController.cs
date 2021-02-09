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
    public class SolutionController : ControllerBase
    {
        private IMongoService _mongoService;
        public SolutionController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("get-solutions")]
        public async Task<IActionResult> GetSolutions()
        {
            var collection = _mongoService.Database.GetCollection<Solution>("solutions");

            var data = await collection.Find(f => true)
                .SortByDescending(s => s.Id)
                .ToListAsync();

            return Ok(data);
        }

        [HttpPost("create-solution")]
        public async Task<IActionResult> Create([FromBody] Solution solution)
        {
            var collection = _mongoService.Database.GetCollection<Solution>("solutions");
            if (!TryValidateModel(solution))
                return BadRequest();

            await collection.InsertOneAsync(solution);


            return Ok(solution);
        }

        [HttpPost("save-solution")]
        public async Task<IActionResult> Save([FromBody] Solution solution)
        {
            var collection = _mongoService.Database.GetCollection<Solution>("solutions");
            await collection.ReplaceOneAsync(
                Builders<Solution>.Filter.Eq(e => e.Id, solution.Id), solution);


            return Ok(new { message = $"Solution {solution.Name} saved successfully" });
        }


        [HttpPost("delete-solution")]
        public async Task<IActionResult> Delete([FromBody] Solution sol)
        {
            var collection = _mongoService.Database.GetCollection<Solution>("solutions");
            await collection.DeleteOneAsync(
                  Builders<Solution>.Filter.Eq(e => e.Id, sol.Id));

            return Ok(new { message = $"Solution {sol.Name} saved successfully" });
        }


    }
}
