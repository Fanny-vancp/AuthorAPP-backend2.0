using MongoDB.Bson.Serialization.Attributes;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class IdentityInfo
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
    }
}
