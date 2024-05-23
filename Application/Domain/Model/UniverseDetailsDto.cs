using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class UniverseDetailsDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("literary_genre")]
        public string LiteraryGenre { get; set; }

        [BsonElement("characters")]
        public List<CharacterReference> Characters { get; set; }
    }
}
