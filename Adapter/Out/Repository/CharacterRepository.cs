using UniverseCreation.API.Adapter.Out.DataAccess;

namespace UniverseCreation.API.Adapter.Out.Repository
{
    public interface ICharacterRepository
    {
        Task<List<Dictionary<string, object>>> MatchAllCharactersByUniverseName(string universeName);
        Task<List<Dictionary<string, object>>> MatchAllCharactersWithHisRelative(string universeName);
        Task<List<Dictionary<string, object>>> SearchCharacterBythename(string searchName, string universeName);
    }

    public class CharacterRepository : ICharacterRepository
    {
        private INeo4jDataAccess _neo4JDataAccess;
        private ILogger<CharacterRepository> _logger;

        public CharacterRepository(INeo4jDataAccess neo4JDataAccess, ILogger<CharacterRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        // search by the name of a character
        public async Task<List<Dictionary<string, object>>> SearchCharacterBythename(string universe, string searchName)
        {
            var query = @"MATCH(univers: Universe { name: $universe })< - [:CAST_FROM] - (character: Character) WHERE character.name CONTAINS $searchName
                        RETURN character{ name: character.name, firstname: character.firstname, lastname: character.lastname} ORDER BY character.name LIMIT 5";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "universe", universe }, { "searchName", searchName } };

            var characters = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "character", parameters);

            return characters;
        }

        // get all characters
        public async Task<List<Dictionary<string, object>>> MatchAllCharactersByUniverseName(string universe)
        {
            var query = @"MATCH(univers: Universe { name: $universe })< - [:CAST_FROM] - (character: Character) RETURN character";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "universe", universe } };

            var characters = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "character", parameters);

            return characters;
        }

        // get a character and his relative
        public async Task<List<Dictionary<string, object>>> MatchAllCharactersWithHisRelative(string universe)
        {
            var query = @"MATCH (univers:Universe { name: $universe })<-[:CAST_FROM]-(character:Character)
                        MATCH (character)-[rel:RELATIVE_TO]->(otherCharacter:Character)
                        RETURN character, rel, otherCharacter";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "universe", universe } };

            var rels = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "rel", parameters);

            var characters = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "character", parameters);

            var result = new List<Dictionary<string, object>>();

            return result;
        }
    }
}
