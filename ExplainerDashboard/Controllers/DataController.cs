using DevExtreme.AspNet.Data;
using ExplainerDashboard.Model;
using ExplainerDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            

            //var collection = _mongoService.Database.GetCollection<Flock>("flocks");
            //var filter = Builders<Flock>.Filter.Eq(s => s.Day, day);
            //var data = collection.Find(filter)
            //    .SortByDescending(s => s.Day)
            //    .ToList();


            var d = GetFlockUpdated(day).Result;
            return DataSourceLoader.Load(d, loadOptions);

        }


        [HttpGet("get-all-flocks")]
        public async Task<IActionResult> GetFlocks()
        {
            //var collection = _mongoService.Database.GetCollection<Flock>("flocks");
            //var data = await collection.Find(f=>true)
            //    .SortByDescending(s => s.Day)
            //    .ToListAsync();
            var d = GetFlocksUpdated().Result;
            return Ok(d);

        }

        public async Task<List<Flock>> GetFlocksUpdated()
        {

            var collection = _mongoService.Database.GetCollection<Flock>("flocks");
            var collectionDecision = _mongoService.Database.GetCollection<Decision>("decisions");

            var dataD = collectionDecision.Find(f => true)
                .SortBy(s => s.Day)
                .ThenBy(s => s.Plant)
                .ToList();

            var prevDecisions = dataD.GroupBy(g => new {g.Day, g.Plant }, (i, k) => new { day = i.Day  ,plant = i.Plant, sum = k.Sum(s => s.Quantity) });

            var dataF = collection.Find(f=>true)
                .SortBy(s => s.PlantName)
                .ToList();

            var data = dataF.Select(f =>
            {
                f.Fcr = Math.Round(f.Fcr, 2);
                f.MortalityRate = Math.Round(f.MortalityRate, 6);

                f.AvgCoefficientVariation = dataF.Where(s => s.Day <= f.Day && s.PlantName == f.PlantName).Average(a => a.CoefficientVariation);

                if (prevDecisions.Any(a => f.Day > a.day && a.plant == f.PlantName))
                {
                    f.LiveChickQuantity = f.LiveChickQuantity - prevDecisions.Where(w => w.day < f.Day && w.plant == f.PlantName).Sum(s=>s.sum);
                }
                return f;
            });

            return data.ToList();

        }

        public async Task<List<Flock>> GetFlockUpdated(int day)
        {

            var collection = _mongoService.Database.GetCollection<Flock>("flocks");
            var collectionDecision = _mongoService.Database.GetCollection<Decision>("decisions");
            var collectionCost = _mongoService.Database.GetCollection<FeedCost>("feedcost");

            var filterD = Builders<Decision>.Filter.Eq(s => s.Day, day);
            var dataD = collectionDecision.Find(f=>true)
                .SortByDescending(s => s.Day)
                .ToList();

            var prevDecisions = dataD.Where(w => w.Day < day).GroupBy(g => g.Plant, (i, k) => new { plant = i, sum = k.Sum(s => s.Quantity) });


            var cost = collectionCost.Find(f => true).ToList();



            var filterF = Builders<Flock>.Filter.Eq(s => s.Day, day);
            var dataF = collection.Find(filterF)
                .SortBy(s => s.PlantName)
                .ToList()
                .Select(s =>
                 {

                     var currentCost = cost.FirstOrDefault(f => f.DayFrom <= day && f.DayTo >= day).CostPerKg;
                     s.FeedConsumption = Math.Round(s.Fcr * s.LiveChickQuantity * s.AverageWeight);
                     s.Cost = currentCost * s.FeedConsumption;
                     s.CostPerBird = s.Cost / s.LiveChickQuantity;
                     return s;
                 });

            var filterP = Builders<Flock>.Filter.Lte(s => s.Day, day);
            var dataP = collection.Find(filterP)
                .SortBy(s => s.PlantName)
                .ToList()
                .Select(s =>
                {


                    var currentCost = cost.FirstOrDefault(f => f.DayFrom <= day && f.DayTo >= day).CostPerKg;
                    s.FeedConsumption = Math.Round(s.Fcr * s.LiveChickQuantity * s.AverageWeight);
                    s.Cost = currentCost * s.FeedConsumption;
                    s.CostPerBird = s.Cost / s.LiveChickQuantity;
                    return s;
                });

            var data = dataF.Select(f=>
            {
               
                f.MortalityRate = Math.Round(f.MortalityRate, 6);
        
                f.TotalFeedConsumption = dataP.Where(s => s.Day <= f.Day && s.PlantName == f.PlantName).Sum(a => a.FeedConsumption);
                f.AvgMortalityRate = Math.Round(dataP.Where(s => s.Day <= f.Day && s.PlantName == f.PlantName).Average(a => a.MortalityRate),6);
                f.AvgCoefficientVariation = Math.Round(dataP.Where(s => s.Day <= f.Day && s.PlantName == f.PlantName).Average(a => a.CoefficientVariation),2);

                f.TotalCost = Math.Round(dataP.Where(s => s.Day <= f.Day && s.PlantName == f.PlantName).Sum(a => a.Cost),4);
                f.TotalCostPerBird = Math.Round(dataP.Where(s => s.Day <= f.Day && s.PlantName == f.PlantName).Sum(a => a.CostPerBird), 4);

              

                if (prevDecisions.Any(a => a.plant == f.PlantName)) {

                    f.LiveChickQuantity = f.LiveChickQuantity - prevDecisions.First(p => p.plant == f.PlantName).sum;
                }

                f.Fcr = f.TotalFeedConsumption / (f.LiveChickQuantity * f.AverageWeight);

                return f;
            });



            return data.ToList();
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


        [HttpGet("intitlize-flocks")]
        public async Task<IActionResult> Initialize()
        {
            var collection = _mongoService.Database.GetCollection<Flock>("flocks");
            await collection.DeleteManyAsync(f=>true);


            var collectionBc = _mongoService.Database.GetCollection<Flock>("flocks-bc");
            var newData = await collectionBc.FindAsync(f => true);
            await collection.InsertManyAsync(newData.ToList());
            return Ok();
        }

        [HttpGet("norm-dist")]
        public async Task<IActionResult> CalculateNormDistGraph(float mean, float stddev)
        {
            float wxmin = 0;
            float wxmax = mean * 2;
            float var = stddev * stddev;
            int width = 500;
            List<PointF> points = new List<PointF>();
            float one_over_2pi =
                (float)(1.0 / (stddev * Math.Sqrt(2 * Math.PI)));

            float dx = (wxmax - wxmin) / width;
            for (float x = wxmin; x <= wxmax; x += dx)
            {
                float y = F(x, one_over_2pi, mean, stddev, var);
                points.Add(new PointF(x, y));
            }

            var dist = points.Where(w => w.Y > 0.01 && w.Y < 0.02);


            return Ok(new { points = points.ToArray().Where(w => w.Y != 0), min = dist.First().X, max = dist.Last().X });
        }
        // The normal distribution function.
        private float F(float x, float one_over_2pi, float mean, float stddev, float var)
        {
            return (float)(one_over_2pi * Math.Exp(-(x - mean) * (x - mean) / (2 * var)));
        }
    }
}
