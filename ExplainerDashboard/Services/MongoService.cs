using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExplainerDashboard.Services
{
    public class MongoDatabaseSettings : IMongoDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string MongoServer { get; set; }
        public int MongoPort { get; set; }
        public string DatabaseName { get; set; }
    }


    public interface IMongoDatabaseSettings
    {
        string ConnectionString { get; set; }
        string MongoServer { get; set; }
        int MongoPort { get; set; }
        string DatabaseName { get; set; }
    }
    public interface IMongoService
    {
        IMongoDatabase Database { get; }

    }

    public class MongoService : IMongoService
    {
        private ILogger _logger;
        public MongoService(IMongoDatabaseSettings settings,
            ILogger<MongoService> logger)
        {
            _logger = logger;

            var mongoSettings = new MongoClientSettings();
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(1);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(1);
            mongoSettings.Server = new MongoServerAddress(settings.MongoServer, settings.MongoPort);
            var client = new MongoClient(mongoSettings);
            Database = client.GetDatabase(settings.DatabaseName);
            _logger.LogInformation("MongoService initilized...");
        }
        public IMongoDatabase Database { get; }
    }
}
