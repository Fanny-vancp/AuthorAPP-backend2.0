using UniverseCreation.API.Adapter.Out.DataAccess;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Adapter.Out.Repository
{
    public class CharacterRepositoryGraph
    {
        private INeo4jDataAccess _neo4JDataAccess;
        private ILogger<CharacterRepositoryGraph> _logger;

        public CharacterRepositoryGraph(INeo4jDataAccess neo4JDataAccess, ILogger<CharacterRepositoryGraph> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        // find the character from his name
        public async Task<List<Dictionary<string, object>>> FindCharacter(string characterName)
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
        public async Task<List<Dictionary<string, object>>> MatchCharactersWithString(string universe, string searchName)
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

        // get all characters from a familyTree
        public async Task<List<Dictionary<string, object>>> MatchAllCharactersFromFamilyTree(string familyTree)
        {
            var query = @"MATCH(familyTree: FamilyTree { name: $familyTree })< - [:FAMILY_FROM] - (character: Character) RETURN character";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "familyTree", familyTree } };

            var characters = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "character", parameters);

            return characters;
        }

        // Creating a relation between a characters and a family Tree
        public async Task<bool> CreateRelationBetweenCharacterAndFamilyTree(string familyTreeName, string characterName)
        {
            if (familyTreeName != null && !string.IsNullOrWhiteSpace(familyTreeName)
                && characterName != null && !string.IsNullOrWhiteSpace(characterName))
            {
                var query = @"MATCH (familyTree: FamilyTree {name: $familyTreeName}),
	                        (character: Character {name: $characterName})
                            CREATE (character)-[:FAMILY_FROM {level: -1}]->(familyTree)";

                IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "familyTreeName", familyTreeName },
                    { "characterName", characterName }
                };

                _logger.LogInformation($"Relation between '{familyTreeName}' and '{characterName}' has been created successfully");

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

        // Deleting a relation between a characters and a family Tree
        public async Task<bool> DeleteRelationBetweenCharacterAndFamilyTree(string familyTreeName, string characterName)
        {
            var query = @"MATCH (character:Character {name: $characterName})-[rel:FAMILY_FROM]->
                        (familyTree:FamilyTree {name: $familyTreeName})
                        DELETE rel";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "familyTreeName", familyTreeName },
                    { "characterName", characterName }
                };

            _logger.LogInformation($"Relation between '{familyTreeName}' and '{characterName}' have been deleted successfully");

            return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);

        }

        // get all Family Tree from a Character
        public async Task<List<Dictionary<string, object>>> MatchAllFamilyTreeFromCharacter(string character)
        {
            var query = @"MATCH (character:Character { name: $character })-[:FAMILY_FROM]->(familyTree:FamilyTree) 
                        RETURN familyTree";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "character", character } };

            var familiesTrees = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "family_tree", parameters);

            return familiesTrees;
        }

        // Deleting a Relation RELATIVE_TO between Characters in a Family Tree
        public async Task<bool> DeleteRelationBetweenCharactersInFamilyTree(string characterName)
        {
            var query = @"MATCH (c:Character {name: $characterName})
                        OPTIONAL MATCH (c)<-[rel:RELATIVE_TO]-(character:Character)
                        OPTIONAL MATCH (c)-[rel2:RELATIVE_TO]->(character2:Character)
                        DELETE rel, rel2";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterName", characterName }
            };

            _logger.LogInformation($"Relation between '{characterName}' and other character have been deleted successfully");

            return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);

        }

        // Creating a Relation RELATIVE_TO between two Characters
        public async Task<bool> CreateRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription)
        {
            var query = @"MATCH (c1: Character { name: $characterName1 }),
                            (c2: Character { name: $characterName2 })
                        CREATE (c1)-[:RELATIVE_TO { description: $relationDescription }]->(c2)";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterName1", characterName1 },
                    { "characterName2", characterName2 },
                    { "relationDescription", relationDescription }
            };

            _logger.LogInformation($"Relation between '{characterName1}' and '{characterName2}' have been created successfully");

            return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
        }

        // Getting a relation RELATIVE_TO between two characters
        public async Task<List<Dictionary<string, object>>> MatchRelationBetweenCharacters(string characterName1, string characterName2)
        {
            var query = @"MATCH (c1:Character {name: $characterName1})-[relation:RELATIVE_TO]->(c2:Character {name: $characterName2})
                        RETURN relation";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterName1", characterName1 },
                    { "characterName2", characterName2 }
            };

            _logger.LogInformation($"Relation between '{characterName1}' and '{characterName2}' have been find successfully");

            var relation = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "relation", parameters);

            if (relation != null && relation.Count == 0)
            {
                relation = null;
            }

            return relation;
        }

        // Deleting a Relation RELATIVE_TO between two Characters
        public async Task<bool> DeleteRelationBetweenCharacters(string characterName1, string characterName2)
        {
            var query = @"MATCH (c1:Character { name: $characterName1 })-[rel1:RELATIVE_TO]->
                            (c2:Character { name: $characterName2 }),
                            (c1)<-[rel2:RELATIVE_TO]-(c2)
                        DELETE rel1, rel2";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterName1", characterName1 },
                    { "characterName2", characterName2 }
            };

            _logger.LogInformation($"Relation between '{characterName1}' and '{characterName2}' have been deleted successfully");

            return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
        }

        // Setting a Relation RELATIVE_TO between two Characters
        public async Task<bool> SetRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription)
        {
            var query = @"MATCH (c1:Character { name: $characterName1 })
                            -[rel:RELATIVE_TO]->
                            (c2:Character { name: $characterName2 })
                        SET rel.description = $relationDescription";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterName1", characterName1 },
                    { "characterName2", characterName2 },
                    { "relationDescription", relationDescription }
            };

            _logger.LogInformation($"Relation between '{characterName1}' and '{characterName2}' have been updated successfully");

            return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
        }

        // Get a list of Characters that had a Relation RELATIVE_TO to a Character
        public async Task<List<Dictionary<string, object>>> MatchCharactersWithRelationFromCharacter(string characterName, string relationDescription)
        {
            var query = @"MATCH (character:Character {name: $characterName})<-[:RELATIVE_TO {description: $relationDescription}]-(relative:Character)
                        RETURN relative";

            IDictionary<string, object> parameters = new Dictionary<string, object> { 
                { "characterName", characterName }, 
                { "relationDescription", relationDescription } 
            };

            var characters = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "relative", parameters);
            if (characters.Count ==0) characters = new List<Dictionary<string, object>>();

            return characters;
        }

        // get all level character from a family tree
        public async Task<List<Dictionary<string, object>>> MatchRelationFamilyFromLevel(string characterName, string familyTreeName)
        {
            var query = @"MATCH (c:Character {name: $characterName})-[relation:FAMILY_FROM]->(FamilyTree {name: $familyTreeName})
                        RETURN relation";

            IDictionary<string, object> parameters = new Dictionary<string, object> { 
                { "characterName", characterName },
                { "familyTreeName", familyTreeName }
            };

            var relationLevel = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "relation", parameters);

            return relationLevel;
        }

        // create a new node character
        public async Task<bool> CreateCharacterNode(CharacterForCreationDto character)
        {
            string characterPseudo = character.Pseudo;
            if (characterPseudo != null && !string.IsNullOrWhiteSpace(characterPseudo))
            {
                var query = @"CREATE (character: Character{name: $characterPseudo})";

                IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterPseudo", characterPseudo }
                };

                _logger.LogInformation($"The character with the name {characterPseudo} has been created successfully in graph database");

                return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
            }
            else
            {
                throw new System.ArgumentNullException(nameof(characterPseudo), "The name of the character must not be null");
            }
        }

        // Setting a level of a character from a family tree
        public async Task<bool> SetLevelFamilyFrom(string characterName, string familyTreeName, int level)
        {
            var query = @"MATCH (c:Character {name: $characterName})-[r:FAMILY_FROM]->(FamilyTree {name: $familyTreeName})
                        SET r.level = $level";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterName", characterName },
                    { "familyTreeName", familyTreeName },
                    { "level", level }
            };

            _logger.LogInformation($"Level of the character {characterName} and the familyTree {familyTreeName} have been updated successfully");

            return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
        }

        // Setting a name of a character
        public async Task<bool> SetCharacterName(string characterName, string newCharacterName)
        {
            var query = @"MATCH (character:Character {name: $characterName})
                        SET character.name = $newCharacterName";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterName", characterName },
                    { "newCharacterName", newCharacterName }
            };

            _logger.LogInformation($"The name of the character {characterName} have been updated with the name {newCharacterName} successfully");

            return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
        }

        // Delete a character node
        public async Task<bool> DeleteCharacter(string characterName)
        {
            var query = @"MATCH (character:Character {name: $characterName})
                        DETACH DELETE character";

            IDictionary<string, object> parameters = new Dictionary<string, object> {
                    { "characterName", characterName }
            };

            _logger.LogInformation($"The character {characterName} have been delete successfully");

            return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
        }

        // get a list of familyTree by a characterName
        public async Task<List<Dictionary<string, object>>> getFamilyTreeByCharacter(string characterName)
        {
            var query = @"MATCH (:Character {name: $characterName})-[:FAMILY_FROM]->(familyTree:FamilyTree)
                        RETURN familyTree";

            IDictionary<string, object> parameters = new Dictionary<string, object> { 
                { "characterName", characterName } 
            };

            var familiesTree = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "familyTree", parameters);

            if (familiesTree != null && familiesTree.Count == 0)
            {
                familiesTree = null;
            }

            return familiesTree;
        }
    }
}
