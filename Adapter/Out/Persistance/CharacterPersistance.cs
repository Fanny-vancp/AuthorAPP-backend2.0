using UniverseCreation.API.Adapter.In.Controllers;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Adapter.Out.Persistance
{
    public class CharacterPersistance : ICharacterPersistance
    {
        private readonly ILogger<CharacterPersistance> _logger;
        private readonly CharacterRepositoryGraph _characterRepositoryGraph;
        private readonly FamilyTreeRepositoryGraph _familyTreeRepositoryGraph;
        private readonly UniverseRepositoryGraph _universeRepositoryGraph;
        private readonly CharacterRepositoryMongo _characterRepositoryMongo;
        private readonly UniverseRepositoryMongo _universeRepositoryMongo;
        
        public CharacterPersistance(ILogger<CharacterPersistance> logger, CharacterRepositoryGraph characterRepositoryGraph, 
            FamilyTreeRepositoryGraph familyTreeRepositoryGraph, UniverseRepositoryGraph universeRepositoryGraph,
            CharacterRepositoryMongo characterRepositoryMongo, UniverseRepositoryMongo universeRepositoryMongo)
        {
            _logger = logger;
            _characterRepositoryGraph = characterRepositoryGraph;
            _familyTreeRepositoryGraph = familyTreeRepositoryGraph;
            _universeRepositoryGraph = universeRepositoryGraph;
            _characterRepositoryMongo = characterRepositoryMongo;
            _universeRepositoryMongo = universeRepositoryMongo;
        }

        public async Task<List<Dictionary<string, object>>> GetCharacterFromString(string universe, string searchName)
        {
            // check if the universe exist
            var universeFound = await _universeRepositoryGraph.FindUniverse(universe);
            if (universeFound == null)
            {
                _logger.LogInformation($"Universe with the name {universe} was'nt found when searching characters from string.");
                throw new InvalidOperationException($"Universe with the name {universe} was'nt found when searching characters from string.");
            }

            var characters = await _characterRepositoryGraph.MatchCharactersWithString(universe, searchName);
            return characters;
        }

        public async Task<List<Dictionary<string, object>>> GetAllCharactersNodeFromUniverseName(string universe)
        {
            // check if the universe exist
            var universeFound = await _universeRepositoryGraph.FindUniverse(universe);
            if (universeFound == null)
            {
                _logger.LogInformation($"Universe with the name {universe} was'nt found when accessing characters from universe.");
                throw new InvalidOperationException($"Universe with the name {universe} was'nt found when accessing characters from universe.");
            }

            var characters = await _characterRepositoryGraph.MatchAllCharactersByUniverseName(universe);
            return characters;
        }

        public async Task<List<Dictionary<string, object>>> GetAllCharactersFromFamilyTree(string family_treeName)
        {
            // check if the family tree exist
            var familyTreeFound = await _familyTreeRepositoryGraph.FindFamilyTree(family_treeName);
            if (familyTreeFound == null)
            {
                _logger.LogInformation($"Family Tree with the name {family_treeName} was'nt found when accessing characters.");
                throw new InvalidOperationException($"Family Tree with the name {family_treeName} was'nt found when accessing characters.");
            }

            var characters = await _characterRepositoryGraph.MatchAllCharactersFromFamilyTree(family_treeName);

            return characters;
        }

        public async Task<bool> ConnectCharacterToFamilyTree(string familyTreeName, string characterName)
        {
            // check if the family tree exist
            var familyTreeFound = await _familyTreeRepositoryGraph.FindFamilyTree(familyTreeName);
            if (familyTreeFound == null)
            {
                _logger.LogInformation($"Family Tree with the name {familyTreeName} was'nt found when adding character in familyTree.");
                throw new InvalidOperationException($"Family Tree with the name {familyTreeName} was'nt found when adding character in familyTree.");
            }

            // check if the character exist
            var characterFound = await _characterRepositoryGraph.FindCharacter(characterName);
            if (characterFound == null)
            {
                _logger.LogInformation($"Character with the name {characterName} was'nt found when adding character in familyTree.");
                throw new InvalidOperationException($"Character with the name {characterName} wasn't found when adding character in familyTree.");
            }

            return await _characterRepositoryGraph.CreateRelationBetweenCharacterAndFamilyTree(familyTreeName, characterName);
        }

        public async Task<bool> DisconnectCharacterToFamilyTree(string familyTreeName, string characterName)
        {
            // check if the family tree exist
            var familyTreeFound = await _familyTreeRepositoryGraph.FindFamilyTree(familyTreeName);
            if (familyTreeFound == null)
            {
                _logger.LogInformation($"Family Tree with the name {familyTreeName} was'nt found when removing character in familyTree.");
                throw new InvalidOperationException($"Family Tree with the name {familyTreeName} was'nt found when removing character in familyTree.");
            }

            // check if the character exist
            var characterFound = await _characterRepositoryGraph.FindCharacter(characterName);
            if (characterFound == null)
            {
                _logger.LogInformation($"Character with the name {characterName} was'nt found when removing character in familyTree.");
                throw new InvalidOperationException($"Character with the name {characterName} wasn't found when removing character in familyTree.");
            }

            // check if the character had other family tree
            var familiesTreesFound = await _characterRepositoryGraph.MatchAllFamilyTreeFromCharacter(characterName);
            if (familiesTreesFound.Count > 1)
            {
                return await _characterRepositoryGraph.DeleteRelationBetweenCharacterAndFamilyTree(familyTreeName, characterName);
            }
            else
            {
                await _characterRepositoryGraph.DeleteRelationBetweenCharactersInFamilyTree(characterName);
                return await _characterRepositoryGraph.DeleteRelationBetweenCharacterAndFamilyTree(familyTreeName, characterName);
            }                
        }

        public async Task<bool> ConnectTwoCharacters(string characterName1, string characterName2, string relationDescription)
        {
            // check if the first character exist
            var characterFound1 = await _characterRepositoryGraph.FindCharacter(characterName1);
            if (characterFound1 == null)
            {
                _logger.LogInformation($"Character with the name {characterName1} was'nt found when connecting two characters in familyTree.");
                throw new InvalidOperationException($"Character with the name {characterName1} wasn't found when connecting two characters in familyTree.");
            }
          
            // check if the second character exist
            var characterFound2 = await _characterRepositoryGraph.FindCharacter(characterName2);
            if (characterFound2 == null)
            {
                _logger.LogInformation($"Character with the name {characterName2} was'nt found when connecting two characters in familyTree.");
                throw new InvalidOperationException($"Character with the name {characterName2} wasn't found when connecting two characters in familyTree.");
            }

            return await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterName1, characterName2, relationDescription);
        }

        public async Task<bool> DisconnectTwoCharacters(string characterName1, string characterName2)
        {
            // check if the first character exist
            var characterFound1 = await _characterRepositoryGraph.FindCharacter(characterName1);
            if (characterFound1 == null)
            {
                _logger.LogInformation($"Character with the name {characterName1} was'nt found when removing relationship between two characters in familyTree.");
                throw new InvalidOperationException($"Character with the name {characterName1} wasn't found when removing relationship between two characters in familyTree.");
            }

            // check if the second character exist
            var characterFound2 = await _characterRepositoryGraph.FindCharacter(characterName2);
            if (characterFound2 == null)
            {
                _logger.LogInformation($"Character with the name {characterName2} was'nt found when removing relationship between two characters in familyTree.");
                throw new InvalidOperationException($"Character with the name {characterName2} wasn't found when removing relationship between two characters in familyTree.");
            }

            return await _characterRepositoryGraph.DeleteRelationBetweenCharacters(characterName1, characterName2 );
        }

        public async Task<bool> FixRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription)
        {
            // check if the first character exist
            var characterFound1 = await _characterRepositoryGraph.FindCharacter(characterName1);
            if (characterFound1 == null)
            {
                _logger.LogInformation($"Character with the name {characterName1} was'nt found when updating relationship between two characters in familyTree.");
                throw new InvalidOperationException($"Character with the name {characterName1} wasn't found when updating relationship between two characters in familyTree.");
            }

            // check if the second character exist
            var characterFound2 = await _characterRepositoryGraph.FindCharacter(characterName2);
            if (characterFound2 == null)
            {
                _logger.LogInformation($"Character with the name {characterName2} was'nt found when updating relationship between two characters in familyTree.");
                throw new InvalidOperationException($"Character with the name {characterName2} wasn't found when updating relationship between two characters in familyTree.");
            }

            return await _characterRepositoryGraph.SetRelationBetweenCharacters(characterName1, characterName2, relationDescription);
        }

        public async Task<List<Dictionary<string, object>>> GetAllRelationForCharacter(string characterName, string relationDescription)
        {
            // check if the universe exist
            var characterFound = await _characterRepositoryGraph.FindCharacter(characterName);
            if (characterFound == null)
            {
                _logger.LogInformation($"Character with the name {characterFound} was'nt found when accessing characters relation {relationDescription}.");
                throw new InvalidOperationException($"Character with the name {characterFound} was'nt found when accessing characters relation {relationDescription}.");
            }

            var characters = await _characterRepositoryGraph.MatchCharactersWithRelationFromCharacter(characterName, relationDescription);
            return characters;
        }

        public async Task<List<Dictionary<string, object>>> GetLevelFamilyTreeForCharacter(string characterName, string familyTreeName)
        {
            // check if the family tree exist
            var familyTreeFound = await _familyTreeRepositoryGraph.FindFamilyTree(familyTreeName);
            if (familyTreeFound == null)
            {
                _logger.LogInformation($"Family Tree with the name {familyTreeName} was'nt found when accessing level character in familyTree.");
                throw new InvalidOperationException($"Family Tree with the name {familyTreeName} was'nt found when accessing level character in familyTree.");
            }

            var level = await _characterRepositoryGraph.MatchRelationFamilyFromLevel(characterName, familyTreeName);
            return level;
        }

        public async Task<List<CharacterDto>> GetAllCharacters(string universeId)
        {
            var universe = await _universeRepositoryMongo.CatchUniverseById(universeId);
            var allCharacters = new List<CharacterDto>();

            if (universe != null && universe.Characters != null)
            {
                foreach (var characterReference in universe.Characters)
                {
                    var character = await _characterRepositoryMongo.CatchCharacterById(characterReference.CharacterId);
                    if (character != null)
                    {
                        allCharacters.Add(character);
                    }
                }
            }

            return allCharacters;
        }

        public async Task<CharacterDetailsDto> GetCharacterById(string characterId)
        {
            var character = await _characterRepositoryMongo.CatchCharacterDetailsById(characterId);
            return character;
        }
    }
}
