using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UniverseCreation.API.Application.Domain.Model
{
    public class CharacterDetailsDto
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

        [BsonElement("identity")]
        public List<IdentityInfo> Identity { get; set; }

        [BsonElement("physical_characteristic")]
        public List<PhysicalCharacteristic> PhysicalCharacteristic { get; set; }

        [BsonElement("relation")]
        public List<RelationInfo> Relation { get; set; }

        [BsonElement("personnality")]
        public List<string> Personnality { get; set; }

        [BsonElement("aptitude")]
        public List<string> Aptitude { get; set; }

        [BsonElement("historic")]
        public string Historic { get; set; }
    }

    public class IdentityInfo
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
    }

    public class PhysicalCharacteristic
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
    }

    public class RelationInfo
    {
        [BsonElement("relationship")]
        public string Relationship { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }
}
