﻿using ExplainerDashboard.Model;
using ExplainerDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExplainerDashboard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private IMongoService _mongoService;
        public ProcessController(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpPost("process-solution")]
        public async Task<IActionResult> StartSolution(Solution solution)
        {
            if(solution.IsProlog)
                PrudensProlog(solution)
            else
                PrudensJava(solution)

        }

        private IActionResult PrudensJava(Solution solution) {

            var collection = _mongoService.Database.GetCollection<Solution>("solutions");


            await collection.ReplaceOneAsync(
                Builders<Solution>.Filter.Eq(e => e.Id, solution.Id), solution);


            var filter = Builders<Solution>.Filter.Eq(s => s.Id, solution.Id);
            var entry = collection.Find(filter);

            var sol = collection.Find(filter)
                                 .SingleOrDefault();

            System.IO.File.WriteAllText("kb.txt", string.Join('\n', sol.Kb.Split('\n').Where(s => !s.StartsWith('#'))));
            System.IO.File.WriteAllText("context.txt", string.Join('\n', sol.Coach.Split('\n').Where(s => !s.StartsWith('#'))));


            var dt = new List<string>();
            Process clientProcess = new Process();
            clientProcess.StartInfo.FileName = @"exec.bat";
            clientProcess.StartInfo.UseShellExecute = false;
            clientProcess.StartInfo.RedirectStandardOutput = true;
            clientProcess.OutputDataReceived += (sender, args) =>
            {

                dt.Add(args.Data);
            };



            clientProcess.Start();
            clientProcess.BeginOutputReadLine();




            clientProcess.WaitForExit();
            int code = clientProcess.ExitCode;
            var dtt = dt.Skip(2).SkipLast(1);

            var raw = string.Join("\n", dtt);


            Regex pattern = new Regex(@"(\[(?:\[??[^\[]*?\]))");
            Match match = pattern.Match(raw);
            if (match.Success)
            {

                var len = match.Groups[0].Value.Length;
                sol.Suggest = raw.Substring(0, len);
                sol.Explain = raw.Substring(len + 1, raw.Length - len - 1);
                sol.RawInference = raw;
            }

            sol.SuggestedPlants = System.IO.File.ReadAllText("moves.txt");
            sol.SuggestedPlantsList = sol.SuggestedPlants.Split('\n')
                .Where(w => !string.IsNullOrEmpty(w)).ToList();

            await collection.ReplaceOneAsync(
                    Builders<Solution>.Filter.Eq(e => e.Id, solution.Id), sol);

            return Ok(sol);
        }
        private IActionResult PrudensProlog(Solution solution)
        {

            var collection = _mongoService.Database.GetCollection<Solution>("solutions");


            await collection.ReplaceOneAsync(
                Builders<Solution>.Filter.Eq(e => e.Id, solution.Id), solution);


            var filter = Builders<Solution>.Filter.Eq(s => s.Id, solution.Id);
            var entry = collection.Find(filter);

            var sol = collection.Find(filter)
                                 .SingleOrDefault();

            System.IO.File.WriteAllText("kb.txt", string.Join('\n', sol.Kb.Split('\n').Where(s => !s.StartsWith('#'))));
            System.IO.File.WriteAllText("context.txt", string.Join('\n', sol.Coach.Split('\n').Where(s => !s.StartsWith('#'))));


            var dt = new List<string>();
            Process clientProcess = new Process();
            clientProcess.StartInfo.FileName = @"exec.bat";
            clientProcess.StartInfo.UseShellExecute = false;
            clientProcess.StartInfo.RedirectStandardOutput = true;
            clientProcess.OutputDataReceived += (sender, args) =>
            {

                dt.Add(args.Data);
            };



            clientProcess.Start();
            clientProcess.BeginOutputReadLine();




            clientProcess.WaitForExit();
            int code = clientProcess.ExitCode;
            var dtt = dt.Skip(2).SkipLast(1);

            var raw = string.Join("\n", dtt);


            Regex pattern = new Regex(@"(\[(?:\[??[^\[]*?\]))");
            Match match = pattern.Match(raw);
            if (match.Success)
            {

                var len = match.Groups[0].Value.Length;
                sol.Suggest = raw.Substring(0, len);
                sol.Explain = raw.Substring(len + 1, raw.Length - len - 1);
                sol.RawInference = raw;
            }

            sol.SuggestedPlants = System.IO.File.ReadAllText("moves.txt");
            sol.SuggestedPlantsList = sol.SuggestedPlants.Split('\n')
                .Where(w => !string.IsNullOrEmpty(w)).ToList();

            await collection.ReplaceOneAsync(
                    Builders<Solution>.Filter.Eq(e => e.Id, solution.Id), sol);

            return Ok(sol);
        }
    }
}
