using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class BookDetailsDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("chapter")]
        public List<Chapter> Chapters { get; set; }

        [BsonElement("scene")]
        public List<Scene> Scenes { get; set; }
    }

    public class Chapter
    {
        [BsonElement("index")]
        public int Index { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
    }

    public class Scene
    {
        [BsonElement("index")]
        public int Index { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
    }
}
