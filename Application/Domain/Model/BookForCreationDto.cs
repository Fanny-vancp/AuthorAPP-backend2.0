using MongoDB.Bson.Serialization.Attributes;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class BookForCreationDto
    {
        [BsonElement("title")]
        public required string Title { get; set; }
    }
}
