using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ProjectHX.Models.Configuration;

namespace ProjectHX.Contexts.MDB
{
    public sealed class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var mongoSection = configuration.GetSection(nameof(MongoDbSettings));
            if (!mongoSection.Exists())
            {
                // Backward compatibility for older config files.
                mongoSection = configuration.GetSection("MongoDb");
            }

            var mongoSettings = mongoSection.Get<MongoDbSettings>() ?? new MongoDbSettings();
            var connectionString = !string.IsNullOrWhiteSpace(mongoSettings.ConnectionString)
                ? mongoSettings.ConnectionString
                : "mongodb://localhost:27017";
            var databaseName = !string.IsNullOrWhiteSpace(mongoSettings.Database)
                ? mongoSettings.Database
                : "newkenya";

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            return _database.GetCollection<TDocument>(collectionName);
        }
    }
}
