using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Adapter.Out.Repository
{
    public class UniverseRepositoryMongo
    {
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly ILogger<UniverseRepositoryMongo> _logger;

        public UniverseRepositoryMongo(IConfiguration configuration, ILogger<UniverseRepositoryMongo> logger)
        {
            _logger = logger;
            try
            {
                var mongoDbConnection = configuration["ApplicationSettings:MongoDBConnection"];
                var databaseName = configuration["ApplicationSettings:MongoDBDatabase"];
                var collectionName = configuration["ApplicationSettings:UniverseCollectionName"];

                _logger.LogInformation("Connecting to MongoDB with connection string: {ConnectionString}", mongoDbConnection);
                var client = new MongoClient(mongoDbConnection);
                var database = client.GetDatabase(databaseName);
                _collection = database.GetCollection<BsonDocument>(collectionName);
                _logger.LogInformation("Successfully connected to MongoDB and obtained the collection.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MongoDB.");
                throw;
            }
        }

        public async Task<List<UniverseDto>> CatchAllUniverse()
        {
            try
            {
                _logger.LogInformation("Fetching all universes from MongoDB.");
                var documents = await _collection.Find(_ => true).ToListAsync();
                var universeList = new List<UniverseDto>();

                if (documents == null || !documents.Any())
                {
                    _logger.LogWarning("No documents found in the universe collection.");
                    return universeList;
                }

                foreach (var document in documents)
                {
                    universeList.Add(ConvertBsonToUniverse(document));
                }

                _logger.LogInformation("Successfully fetched {Count} universes.", universeList.Count);
                return universeList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all universes.");
                throw;
            }
        }

        public async Task<UniverseDto> CatchUniverseById(string universeId)
        {
            try
            {
                _logger.LogInformation("Fetching universe by ID: {UniverseId}", universeId);
                var universeCursor = await _collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(universeId)));
                var document = universeCursor.FirstOrDefault();
                var universe = ConvertBsonToUniverse(document);

                if (universe == null)
                {
                    _logger.LogWarning("No universe found with ID: {UniverseId}", universeId);
                    return new UniverseDto();
                }

                _logger.LogInformation("Successfully fetched universe with ID: {UniverseId}", universeId);
                return universe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching universe by ID.");
                throw;
            }
        }

        private UniverseDto ConvertBsonToUniverse(BsonDocument document)
        {
            if (document == null) return null;

            var charactersArray = document["characters"].AsBsonArray;

            return new UniverseDto
            {
                Id = document["_id"].AsObjectId.ToString(),
                Name = document["name"].AsString,
                LiteraryGenre = document["literary_genre"].AsString,
                Characters = charactersArray.Select(c => new CharacterReference
                {
                    CharacterId = c["character_id"].AsObjectId.ToString()
                }).ToList()
            };
        }
    }
}