﻿using System.Runtime.InteropServices;
using UniverseCreation.API.Adapter.In.Controllers;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.Out;
using System.Linq;

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
                if (await _characterRepositoryGraph.DeleteRelationBetweenCharactersInFamilyTree(characterName) == false ||
                   await _characterRepositoryGraph.DeleteRelationBetweenCharacterAndFamilyTree(familyTreeName, characterName) == false )
                {
                    return false;
                }
                return true;
            }                
        }

        public async Task<bool> ConnectTwoCharacters(string characterName1, string characterName2, string relationDescription, List<CharacterNodeDto> characters,  string familyTreeName)
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

            if (relationDescription == "Enfant" || relationDescription == "Parent")
            {
                CharacterNodeDto characterParent;
                CharacterNodeDto characterEnfant;
                if (relationDescription == "Parent") {
                    characterParent = characters.FirstOrDefault(node => node.name == characterName1);
                    characterEnfant = characters.FirstOrDefault(node => node.name == characterName2);
                } else
                {
                    characterParent = characters.FirstOrDefault(node => node.name == characterName2);
                    characterEnfant = characters.FirstOrDefault(node => node.name == characterName1);
                }

                // creation of a parent-children relation
                if (characterEnfant.parents != null && characterEnfant.parents.Count >= 2)
                {
                    int existingParentsCount = characterEnfant.parents.Count(parent => characters.Any(c => c.name == parent));
                    if (existingParentsCount >= 2)
                    {
                        // the character already had two parents in the same familyTree
                        return false;
                    }

                    if (characterEnfant.level == -1)
                    {
                        int newLevel = characterParent.level + 1;
                        await _characterRepositoryGraph.SetLevelFamilyFrom(characterEnfant.name, familyTreeName, newLevel);
                        var relationCreated1 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterEnfant.name, characterParent.name, "Enfant");
                        var relationCreated2 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterParent.name, characterEnfant.name, "Parent");

                        if (relationCreated1 == true && relationCreated2 == true) { return true; }
                        else { return false; }
                    }
                    else if (characterParent.level == -1)
                    {
                        if (characterEnfant.level == 0)
                        {
                            foreach (var character in characters)
                            {
                                if (character.level != -1)
                                {
                                    int newLevel = character.level + 1;
                                    await _characterRepositoryGraph.SetLevelFamilyFrom(character.name, familyTreeName, character.level);
                                }
                            }
                            await _characterRepositoryGraph.SetLevelFamilyFrom(characterEnfant.name, familyTreeName, 0);
                        }
                        else { return false; }

                        var relationCreated1 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterEnfant.name, characterParent.name, "Enfant");
                        var relationCreated2 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterParent.name, characterEnfant.name, "Parent");

                        if (relationCreated1 == true && relationCreated2 == true) { return true; }
                        else { return false; }

                    }
                    else { return false; }
                }
            }

            // creation of a relation married-divoced-couple-amant
            if (relationDescription == "Marrié" || relationDescription == "En couple" || 
                relationDescription == "Divorcé" || relationDescription == "amant")
            {
                CharacterNodeDto characterToAdd = characters.FirstOrDefault(character =>
                    (character.name == characterName1 || character.name == characterName2) && character.level == -1);
                CharacterNodeDto characterIn = characters.FirstOrDefault(character =>
                    (character.name == characterName1 || character.name == characterName2) && character.level != -1);

                await _characterRepositoryGraph.SetLevelFamilyFrom(characterToAdd.name, familyTreeName, characterIn.level);

                var relationCreated1 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterToAdd.name, characterIn.name, relationDescription);
                var relationCreated2 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterIn.name, characterToAdd.name, relationDescription);

                if (relationCreated1 == true && relationCreated2 == true) { return true; }
                else { return false; }
            }

            return false;
        }

        public async Task<bool> DisconnectTwoCharacters(string characterName1, string characterName2, string familyTreeName, List<CharacterNodeDto> characters)
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

            // check if a relation exist
            var relation = await _characterRepositoryGraph.MatchRelationBetweenCharacters(characterName1, characterName2);
            if (relation == null)
            {
                _logger.LogInformation($"Relation between the character {characterName1} and the character {characterName2} was'nt found when getting relationship ");
                throw new InvalidOperationException($"Relation between the character {characterName1} and the character {characterName2} was'nt found when getting relationship ");
            }

            List<string> relationDescriptions = new List<string>();

            if (relation != null && relation.Count > 0)
            {
                foreach (var rel in relation)
                {
                    if (rel.ContainsKey("description") && rel["description"] is string)
                    {
                        relationDescriptions.Add((string)rel["description"]);
                    }
                }
            }

            string relationDescription = relationDescriptions[0];

            if (relationDescription == "Parent" || relationDescription == "Enfant")
            {
                // find who is the kid
                string childCharacter;
                if (relationDescription == "Enfant") { childCharacter = characterName1; }
                else { childCharacter = characterName2; }
                CharacterNodeDto childNode = characters.FirstOrDefault(node => node.name == childCharacter);

                if (childNode != null)
                {
                    _ = SetLevelToAllDescendantInStack(childNode, characters, familyTreeName);
                }
                
                // remove the two relation
                var removeRelation1 = await _characterRepositoryGraph.DeleteRelationBetweenCharacters(characterName1, characterName2);
                var removeRelation2 = await _characterRepositoryGraph.DeleteRelationBetweenCharacters(characterName2, characterName1);

                if (removeRelation1 == true && removeRelation2 == true) { return true; }
            }

            if (relationDescription == "Marrié" || relationDescription == "Divorcé" || relationDescription == "En couple")
            {
                var characterToRemoveName = "";
                CharacterNodeDto character1 = characters.FirstOrDefault(node => node.name == characterName1);
                CharacterNodeDto character2 = characters.FirstOrDefault(node => node.name == characterName2);

                // check if a character had parents
                if (character1.parents != null)
                {
                    foreach (var parentName in character1.parents)
                    {
                        // check if the parent is in characters family tree
                        if (characters.Any(c => c.name == parentName))
                        {
                            characterToRemoveName = character2.name;
                        }
                    }
                }
                else if (character2.parents != null)
                {
                    foreach (var parentName in character2.parents)
                    {
                        // check if the parent is in characters family tree
                        if (characters.Any(c => c.name == parentName))
                        {
                            characterToRemoveName = character1.name;
                        }
                    }
                }
                else
                {
                    characterToRemoveName = character2.name;
                }

                CharacterNodeDto characterToRemove = characters.FirstOrDefault(node => node.name == characterToRemoveName);

                if (characterToRemove != null)
                {
                    if (characterToRemove.children != null)
                    {
                        foreach (string childName in characterToRemove.children)
                        {
                            if (characters.Any(c => c.name == childName && c.level != -1))
                            {
                                CharacterNodeDto child = characters.FirstOrDefault(node => node.name == childName);
                                if (child != null)
                                {
                                    await SetLevelToAllDescendantInStack(child, characters, familyTreeName);
                                }
                            }
                        }
                    }
                    await _characterRepositoryGraph.SetLevelFamilyFrom(characterToRemove.name, familyTreeName, -1);
                }

                // remove the two relation
                var removeRelation1 = await _characterRepositoryGraph.DeleteRelationBetweenCharacters(characterName1, characterName2);
                var removeRelation2 = await _characterRepositoryGraph.DeleteRelationBetweenCharacters(characterName2, characterName1);

                if (removeRelation1 == true && removeRelation2 == true) { return true; }

            }


            return false;
        }

        private async Task SetLevelToAllDescendantInStack(CharacterNodeDto character, List<CharacterNodeDto> characters, string familyTreeName)
        {
            character.level = -1;
            await _characterRepositoryGraph.SetLevelFamilyFrom(character.name, familyTreeName, -1);

            if (character.married != null)
            {
                foreach (string spouseName in character.married)
                {
                    if (characters.Any(c => c.name == spouseName && c.level != -1))
                    {
                        CharacterNodeDto spouse = characters.FirstOrDefault(node => node.name == spouseName);
                        if (spouse != null)
                        {
                            await SetLevelToAllDescendantInStack(spouse, characters, familyTreeName);
                        }
                    }
                }
            }

            if (character.children != null)
            {
                foreach (string childName in character.children)
                {
                    if (characters.Any(c => c.name == childName && c.level != -1))
                    {
                        CharacterNodeDto child = characters.FirstOrDefault(node => node.name == childName);
                        if (child != null)
                        {
                            await SetLevelToAllDescendantInStack(child, characters, familyTreeName);
                        }
                    }
                }
            }            
        }

        private async Task SetLevelToAllDescendant(CharacterNodeDto character, List<CharacterNodeDto> characters, string familyTreeName, int level)
        {
            character.level = level;
            await _characterRepositoryGraph.SetLevelFamilyFrom(character.name, familyTreeName, level);

            if (character.married != null)
            {
                foreach (string spouseName in character.married)
                {
                    if (characters.Any(c => c.name == spouseName && c.level == -1))
                    {
                        CharacterNodeDto spouse = characters.FirstOrDefault(node => node.name == spouseName);
                        if (spouse != null)
                        {
                            await SetLevelToAllDescendant(spouse, characters, familyTreeName, level);
                        }
                    }
                }
            }

            if (character.children != null)
            {
                foreach (string childName in character.children)
                {
                    if (characters.Any(c => c.name == childName && c.level == -1))
                    {
                        CharacterNodeDto child = characters.FirstOrDefault(node => node.name == childName);
                        if (child != null)
                        {
                            await SetLevelToAllDescendant(child, characters, familyTreeName, level + 1);
                        }
                    }
                }
            }
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

        public async Task<bool> AddNewCharacter(string idUniverse, CharacterForCreationDto character)
        {
            string idCharacter = await _characterRepositoryMongo.InsertCharacter(character);
            await _universeRepositoryMongo.AddCharacterToUniverse(idUniverse, idCharacter);

            await _characterRepositoryGraph.CreateCharacterNode(character);
            return await _universeRepositoryGraph.AddCharacterToUniverse(character);
        }
    }
}
