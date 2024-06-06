using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class BookDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }
    }
}
