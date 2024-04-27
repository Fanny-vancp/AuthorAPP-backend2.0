using UniverseCreation.API.Adapter.Out.DataAccess;

namespace UniverseCreation.API.Adapter.Out.Repository
{
    public interface ICharacterRepositoryGraph
    {
        Task<List<Dictionary<string, object>>> MatchAllCharactersByUniverseName(string universeName);
        Task<List<Dictionary<string, object>>> MatchAllCharactersFromFamilyTree(string familyTree);
        //Task<List<Dictionary<string, object>>> MatchAllCharactersWithHisRelative(string universeName);
        Task<List<Dictionary<string, object>>> SearchCharacterBythename(string searchName, string universeName);
        
        Task<List<Dictionary<string, object>>> FindCharacter(string universeName);
        Task<bool> AddCharacterToFamilyTree(string familyTreeName, string characterName);
    }

    public class CharacterRepositoryGraph : ICharacterRepositoryGraph
    {
        private INeo4jDataAccess _neo4JDataAccess;
        private ILogger<CharacterRepositoryGraph> _logger;

        public CharacterRepositoryGraph(INeo4jDataAccess neo4JDataAccess, ILogger<CharacterRepositoryGraph> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        // find the character from his name
        public async Task<List<Dictionary<string, object>>> FindCharacter(string  characterName)
        {
            var query = @"MATCH (character: Character {name: $characterName})
                          RETURN character";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "characterName", characterName } };

            var character = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "character", parameters);

            if (character != null && character.Count == 0)
            {
                character = null;
            }

            return character;
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
        /*public async Task<List<Dictionary<string, object>>> MatchAllCharactersWithHisRelative(string universe)
        {
            var query = @"MATCH (univers:Universe { name: $universe })<-[:CAST_FROM]-(character:Character)
                        MATCH (character)-[rel:RELATIVE_TO]->(otherCharacter:Character)
                        RETURN character, rel, otherCharacter";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "universe", universe } };

            var rels = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "rel", parameters);

            var characters = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "character", parameters);

            var result = new List<Dictionary<string, object>>();

            return result;
        }*/

        // get all characters from a familyTree
        public async Task<List<Dictionary<string, object>>> MatchAllCharactersFromFamilyTree(string familyTree)
        {
            var query = @"MATCH(familyTree: FamilyTree { name: $familyTree })< - [:FAMILY_FROM] - (character: Character) RETURN character";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "familyTree", familyTree } };

            var characters = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "character", parameters);

            return characters;
        }

        // Creatinf a relation between a characters and a family Tree
        public async Task<bool> AddCharacterToFamilyTree(string familyTreeName, string characterName)
        {
            if (familyTreeName != null && !string.IsNullOrWhiteSpace(familyTreeName)
                && characterName != null && !string.IsNullOrWhiteSpace(characterName))
            {
                var query = @"MATCH (familyTree: FamilyTree {name: $familyTreeName}),
	                        (character: Character {name: $characterName})
                            CREATE (character)-[:FAMILY_FROM]->(familyTree)";

                IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "familyTreeName", familyTreeName },
                    { "characterName", characterName }
                };

                _logger.LogInformation($"Relation between '{familyTreeName}' and '{characterName}' created successfully");

                return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
            }
            else if (familyTreeName == null && string.IsNullOrWhiteSpace(familyTreeName))
            {
                throw new System.ArgumentNullException(nameof(familyTreeName), "FamilyTreeName must not be null");
            }
            else
            {
                throw new System.ArgumentNullException(nameof(characterName), "Character must not be null");
            }
        }
    }
}
