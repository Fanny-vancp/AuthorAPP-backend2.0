using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class CharacterReference
    {
        [BsonElement("character_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CharacterId { get; set; }
    }
}
