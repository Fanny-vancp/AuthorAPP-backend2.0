using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class BookReference
    {
        [BsonElement("book_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; }
    }
}
