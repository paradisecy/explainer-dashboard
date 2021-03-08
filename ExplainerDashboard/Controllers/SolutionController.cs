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
        private DataController _dataCtrl;
        public SolutionController(IMongoService mongoService)
        {
            _mongoService = mongoService;
            _dataCtrl = new DataController(mongoService);
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

        [HttpGet("generate-context")]
        public async Task<IActionResult> GenerateContext(int day,string solutionId)
        {
            var collectionSol = _mongoService.Database.GetCollection<Solution>("solutions");

            var sol = collectionSol.Find(f => f.Id == solutionId).FirstOrDefault();


            var data = _dataCtrl.GetFlockUpdated(day).Result;
            var context = new List<string>();

            data.ForEach(entry =>
            {

                var plant = entry.PlantName;
                context.Add($"quantity({entry.LiveChickQuantity},{plant});");
                context.Add($"fcr({Math.Round(entry.Fcr,2)},{plant});");
                context.Add($"weight({Math.Round(entry.AverageWeight,2)},{plant});");
                context.Add($"mortality({Math.Round(entry.MortalityRate, 2)},{plant});");
                context.Add($"cv({entry.VarationClass},{plant});");

            });

            var cnt = string.Join('\n', context);
            sol.Coach = cnt;
            var res = Save(sol).Result;

            return Ok(new { result = cnt });
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
