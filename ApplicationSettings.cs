namespace UniverseCreation.API
{
    public class ApplicationSettings
    {
        public Uri Neo4jConnection { get; set; } = null!;
        public string Neo4jUser { get; set; } = null!;
        public string Neo4jPassword { get; set; } = null!;
        public string Neo4jDatabase { get; set; } = null!;

        public string MongoDBConnection { get; set; } = null!;
        public string MongoDBDatabase { get; set; } = null!;
        public string UniverseCollectionName { get; set; } = null!;
        public string CharacterCollectionName { get; set; } = null!;
    }
}
