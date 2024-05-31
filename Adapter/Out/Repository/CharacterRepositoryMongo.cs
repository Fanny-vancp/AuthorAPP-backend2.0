using MongoDB.Bson;
using MongoDB.Driver;
using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Adapter.Out.Repository
{
    public class CharacterRepositoryMongo
    {
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly ILogger<CharacterRepositoryMongo> _logger;

        public CharacterRepositoryMongo(IConfiguration configuration, ILogger<CharacterRepositoryMongo> logger)
        {
            _logger = logger;
            try
            {
                var mongoDbConnection = configuration["ApplicationSettings:MongoDBConnection"];
                var databaseName = configuration["ApplicationSettings:MongoDBDatabase"];
                var collectionName = configuration["ApplicationSettings:CharacterCollectionName"];

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

        public async Task<List<CharacterDto>> CatchAllCharacters()
        {
            try
            {
                _logger.LogInformation("Fetching all characters from MongoDB with selected properties.");

                var projection = Builders<BsonDocument>.Projection
                    .Include("_id")
                    .Include("last_name")
                    .Include("first_name")
                    .Include("pseudo")
                    .Include("images");

                var documents = await _collection
                    .Find(_ => true)
                    .Project(projection)
                    .ToListAsync();

                var characterList = new List<CharacterDto>();

                if (documents == null || !documents.Any())
                {
                    _logger.LogWarning("No documents found in the character collection.");
                    return characterList;
                }

                foreach (var document in documents)
                {
                    characterList.Add(new CharacterDto
                    {
                        Id = document["_id"].AsObjectId.ToString(),
                        LastName = document["last_name"].AsString,
                        FirstName = document["first_name"].AsString,
                        Pseudo = document["pseudo"].AsString,
                        Images = document["images"].AsBsonArray.Select(image => image.AsString).ToList()
                    });
                }

                _logger.LogInformation("Successfully fetched {Count} characters.", characterList.Count);
                return characterList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all characters.");
                throw;
            }
        }

        public async Task<CharacterDto> CatchCharacterById(string characterId)
        {
            try
            {
                _logger.LogInformation("Fetching character by ID: {characterId}", characterId);
                var universeCursor = await _collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(characterId)));
                var document = universeCursor.FirstOrDefault();
                var character = ConvertBsonToCharacter(document);

                if (character == null)
                {
                    _logger.LogWarning("No universe found with ID: {characterId}", characterId);
                    return new CharacterDto();
                }

                _logger.LogInformation("Successfully fetched character with ID: {UniverseId}", characterId);
                return character;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching character by ID.");
                throw;
            }
        }

        public async Task<CharacterDetailsDto> CatchCharacterDetailsById(string characterId)
        {
            try
            {
                _logger.LogInformation("Fetching character by ID: {characterId}", characterId);
                var universeCursor = await _collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(characterId)));
                var document = universeCursor.FirstOrDefault();
                var character = ConvertBsonToCharacterDetails(document);

                if (character == null)
                {
                    _logger.LogWarning("No universe found with ID: {characterId}", characterId);
                    return new CharacterDetailsDto();
                }

                _logger.LogInformation("Successfully fetched character with ID: {UniverseId}", characterId);
                return character;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching character by ID.");
                throw;
            }
        }

        public async Task<String> InsertCharacter(CharacterForCreationDto character)
        {
            try
            {
                _logger.LogInformation("Creating a new character.");
                BsonArray images = new BsonArray();
                BsonArray relation = new BsonArray();
                BsonArray physical_characteristic = new BsonArray();
                BsonArray identity = new BsonArray();
                BsonArray personnality = new BsonArray();
                BsonArray aptitude = new BsonArray();


                var document = new BsonDocument
                {
                    { "last_name", "" },
                    { "first_name", "" },
                    { "pseudo", character.Pseudo },
                    { "images", images },
                    { "identity", identity },
                    { "physical_characteristic", physical_characteristic },
                    { "relation", relation },
                    { "personnality", personnality },
                    { "aptitude", aptitude },
                    { "historic", "" }
                };

                await _collection.InsertOneAsync(document);

                var characterId = document["_id"].AsObjectId.ToString();
                _logger.LogInformation("Successfully created character with ID: {CharacterId}", characterId);

                return characterId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a character.");
                throw;
            }
        }



        private CharacterDto ConvertBsonToCharacter(BsonDocument document)
        {
            if (document == null) return null;


            return new CharacterDto
            {
                Id = document["_id"].AsObjectId.ToString(),
                LastName = document["last_name"].AsString,
                FirstName = document["first_name"].AsString,
                Pseudo = document["pseudo"].AsString,
                Images = document["images"].AsBsonArray.Select(image => image.AsString).ToList()
            };
        }

        private CharacterDetailsDto ConvertBsonToCharacterDetails(BsonDocument document)
        {
            if (document == null) return null;


            return new CharacterDetailsDto
            {
                Id = document["_id"].AsObjectId.ToString(),
                LastName = document["last_name"].AsString,
                FirstName = document["first_name"].AsString,
                Pseudo = document["pseudo"].AsString,
                Images = document["images"].AsBsonArray.Select(image => image.AsString).ToList(),
                Identity = document["identity"].AsBsonArray.Select(identity => new IdentityInfo
                {
                    Title = identity["title"].AsString,
                    Value = identity["value"].AsString
                }).ToList(),
                PhysicalCharacteristic = document["physical_characteristic"].AsBsonArray.Select(pc => new PhysicalCharacteristic
                {
                    Title = pc["title"].AsString,
                    Value = pc["value"].AsString
                }).ToList(),
                Relation = document["relation"].AsBsonArray.Select(relation => relation.AsString).ToList(),
                Personnality = document["personnality"].AsBsonArray.Select(personality => personality.AsString).ToList(),
                Aptitude = document["aptitude"].AsBsonArray.Select(aptitude => aptitude.AsString).ToList(),
                Historic = document["historic"].AsString
            };
        }
    }
}
