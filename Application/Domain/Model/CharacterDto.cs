using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class CharacterDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("last_name")]
        public string LastName { get; set; }

        [BsonElement("first_name")]
        public string FirstName { get; set; }

        [BsonElement("pseudo")]
        public string Pseudo { get; set; }

        [BsonElement("images")]
        public List<string> Images { get; set; }

    }
}
