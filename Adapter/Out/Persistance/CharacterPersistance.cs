using System.Runtime.InteropServices;
using UniverseCreation.API.Adapter.In.Controllers;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.Out;
using System.Linq;
using MongoDB.Bson;
using System.ComponentModel;

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

        public async Task<bool> ConnectCharacterToFamilyTree(string familyTreeName, string characterName, List<CharacterNodeDto> familyMembers)
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


            // if the character had any child who are already in the family
            // and this child had already two parents in this family
            var enfantCharacter = await _characterRepositoryGraph.MatchCharactersWithRelationFromCharacter(characterName, "Parent");
            if (enfantCharacter != null)
            {
                var parentsCharacter = await _characterRepositoryGraph.MatchCharactersWithRelationFromCharacter(characterName, "enfant");
                var parentsCharacterNode = await transformCharacterInCharacterNodeDto(parentsCharacter, familyTreeName);
                if (parentsCharacterNode != null && parentsCharacterNode.Count >= 2)
                {
                    int characterParentIn = 0;
                    foreach (var character in parentsCharacterNode)
                    {
                        // Check if the character is in familyMember
                        if (familyMembers.Any(fm => fm.name == character.name)) 
                        {
                            characterParentIn++;
                        }
                    }

                    if (characterParentIn >= 2)
                    {
                        return false;
                    }
                }
            }        

            var addCharacter = await _characterRepositoryGraph.CreateRelationBetweenCharacterAndFamilyTree(familyTreeName, characterName);

            // if the character added it's the first in the familyTree
            bool setLevel;
            if(familyMembers.Count == 0) 
            { 
                setLevel = await _characterRepositoryGraph.SetLevelFamilyFrom(characterName, familyTreeName, 0);
            } else { setLevel = true; }

            if (addCharacter == true && setLevel == true) { return true; }
            else { return false; } 
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
            return await _characterRepositoryGraph.DeleteRelationBetweenCharacterAndFamilyTree(familyTreeName, characterName) ;              
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

            // Check if the relation already exist 
            var relationsExist = await _characterRepositoryGraph.MatchRelationBetweenCharacters(characterName1, characterName2);
            if (relationsExist != null)
            {
                _logger.LogInformation($"Characters with the name {characterName1} and {characterName2} had already a relation in the FamilyTree with the name {familyTreeName}.");
                throw new InvalidOperationException($"Characters with the name {characterName1} and {characterName2} had already a relation in the FamilyTree with the name {familyTreeName}.");
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
                }


                if (characterEnfant.level == -1)
                {
                    int newLevel = characterParent.level + 1;
                    //await _characterRepositoryGraph.SetLevelFamilyFrom(characterEnfant.name, familyTreeName, newLevel);
                    await SetLevelToAllDescendant(characterEnfant, characters, familyTreeName, newLevel);
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
                                await _characterRepositoryGraph.SetLevelFamilyFrom(character.name, familyTreeName, newLevel);
                            }
                        }
                        await _characterRepositoryGraph.SetLevelFamilyFrom(characterParent.name, familyTreeName, 0);
                    }
                    else { return false; }

                    var relationCreated1 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterEnfant.name, characterParent.name, "Enfant");
                    var relationCreated2 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterParent.name, characterEnfant.name, "Parent");

                    if (relationCreated1 == true && relationCreated2 == true) { return true; }
                    else { return false; }

                }
                else if (characterEnfant.level == characterParent.level + 1)
                {
                    var parentOne = characters.FirstOrDefault(fm => characterEnfant.parents.Contains(fm.name));

                    if (parentOne != null)
                    {
                        // chech if there is a relation between the parent already on the tree and the new parent
                        var relation = await _characterRepositoryGraph.MatchRelationBetweenCharacters(characterParent.name, parentOne.name);
                        if (relation != null)
                        {
                            var relationCreated1 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterEnfant.name, characterParent.name, "Enfant");
                            var relationCreated2 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterParent.name, characterEnfant.name, "Parent");

                            if (relationCreated1 == true && relationCreated2 == true) { return true; }
                            else { return false; }
                        }
                    }
                    else { return false; }
                    
                }
                else { return false; }
            }

            // creation of a relation married-divoced-couple-amant
            if (relationDescription == "Marrié" || relationDescription == "En couple" || 
                relationDescription == "Divorcé" || relationDescription == "amant")
            {
                CharacterNodeDto characterToAdd = characters.FirstOrDefault(character =>
                    (character.name == characterName1 || character.name == characterName2) && character.level == -1);
                CharacterNodeDto characterIn = characters.FirstOrDefault(character =>
                    (character.name == characterName1 || character.name == characterName2) && character.level != -1);

                // check if the character who is already here does not have a horizontale relation
                // with an other character present in the family
                bool hasMarriedRelation = characters.Any(fm => characterIn.married != null && characterIn.married.Contains(fm.name));
                bool hasDivorcedRelation = characters.Any(fm => characterIn.divorced != null && characterIn.divorced.Contains(fm.name));
                bool hasAmantRelation = characters.Any(fm => characterIn.amant != null && characterIn.amant.Contains(fm.name));
                bool hasCoupleRelation = characters.Any(fm => characterIn.couple != null && characterIn.couple.Contains(fm.name));

                if (!hasMarriedRelation && !hasDivorcedRelation &&
                    !hasCoupleRelation && !hasAmantRelation)
                {
                    await _characterRepositoryGraph.SetLevelFamilyFrom(characterToAdd.name, familyTreeName, characterIn.level);

                    var relationCreated1 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterToAdd.name, characterIn.name, relationDescription);
                    var relationCreated2 = await _characterRepositoryGraph.CreateRelationBetweenCharacters(characterIn.name, characterToAdd.name, relationDescription);

                    if (relationCreated1 == true && relationCreated2 == true) { return true; }
                    else { return false; }
                }
                else return false;
                
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

                // check if the child had two parents in the tree
                int validParentCount = 0;

                foreach (var character in characters)
                {
                    if ((character.children.Contains(childCharacter)) && character.level != -1)
                    {
                        validParentCount++;
                    }
                }

                if (validParentCount == 1)
                {
                    await SetLevelToAllDescendantInStack(childNode, characters, familyTreeName);
                }
                
                // remove the two relation
                var removeRelation1 = await _characterRepositoryGraph.DeleteRelationBetweenCharacters(characterName1, characterName2);
                var removeRelation2 = await _characterRepositoryGraph.DeleteRelationBetweenCharacters(characterName2, characterName1);

                if (removeRelation1 == true && removeRelation2 == true) { return true; }
            }

            if (relationDescription == "Marrié" || relationDescription == "Divorcé" || relationDescription == "En couple")
            {
                var characterToRemoveName = "";
                var characterToKeepName = "";
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
                            characterToKeepName = character1.name;
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
                            characterToKeepName = character2.name;
                        }
                    }
                }
                else
                {
                    characterToRemoveName = character2.name;
                    characterToKeepName = character1.name;
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
                                    if (!child.parents.Contains(characterToKeepName))
                                    {
                                        await SetLevelToAllDescendantInStack(child, characters, familyTreeName);
                                    }
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
            var addCharacter = await _universeRepositoryMongo.AddCharacterToUniverse(idUniverse, idCharacter);

            var addNodeCharacter = await _characterRepositoryGraph.CreateCharacterNode(character);
            var AddCharacterToUniverse = await _universeRepositoryGraph.AddCharacterToUniverse(character);

            if(addCharacter == true && addNodeCharacter == true) { return true; }
            else { return false; }
        }

        public async Task<bool> ReformCharacter(CharacterDetailsDto character, string characterName)
        {
            var updateCharacter = await _characterRepositoryMongo.UpdateCharacter(character);
            string characterDetailsName = character.FirstName + " " + character.LastName;

            var updateNameCharacter = true;
            if (characterDetailsName != characterName)
            {
                Console.WriteLine("changement name");
                updateNameCharacter = await _characterRepositoryGraph.SetCharacterName(characterName, characterDetailsName);
            }

            if (updateCharacter == true && updateNameCharacter == true) { return true; }
            else { return false; }
        }

        private async Task<List<CharacterNodeDto>> transformCharacterInCharacterNodeDto(List<Dictionary<string, object>> characters, string family_treeName)
        {
            var characterNodeDtos = new List<CharacterNodeDto>();

            foreach (var character in characters)
            {
                var characterName = character["name"].ToString();

                var childrenDict = await GetAllRelationForCharacter(characterName, "Enfant");
                var parentsDict = await GetAllRelationForCharacter(characterName, "Parent");
                var marriedDict = await GetAllRelationForCharacter(characterName, "Marrié");
                var divorcedDict = await GetAllRelationForCharacter(characterName, "Divorcé");
                var coupleDict = await GetAllRelationForCharacter(characterName, "En couple");
                var amantDict = await GetAllRelationForCharacter(characterName, "Amant");
                var levelDict = await GetLevelFamilyTreeForCharacter(characterName, family_treeName);

                var children = childrenDict.Select(child => child["name"].ToString()).ToList();
                var parents = parentsDict.Select(parent => parent["name"].ToString()).ToList();
                var married = marriedDict.Select(marriage => marriage["name"].ToString()).ToList();
                var divorced = divorcedDict.Select(divorce => divorce["name"].ToString()).ToList();
                var couple = coupleDict.Select(couple => couple["name"].ToString()).ToList();
                var amant = amantDict.Select(couple => couple["name"].ToString()).ToList();
                var level = levelDict.Select(level => Convert.ToInt32(level["level"])).FirstOrDefault();

                var characterNodeDto = new CharacterNodeDto
                {
                    name = characterName,
                    children = children,
                    parents = parents,
                    married = married,
                    divorced = divorced,
                    couple = couple,
                    amant = amant,
                    level = level,
                };

                characterNodeDtos.Add(characterNodeDto);
            }

            return characterNodeDtos;
        }

        public async Task<bool> RemoveCharacter(string universeId, string characterId, string characterName )
        {
            var familiesFree = await _characterRepositoryGraph.getFamilyTreeByCharacter(characterName);
            if (familiesFree != null)
            {
                foreach (var family in familiesFree)
                {
                    string familyName = family["name"].ToString();

                    var characters = await _characterRepositoryGraph.MatchAllCharactersFromFamilyTree(familyName);

                    if (characters.Count != 0)
                    {
                        var charactersNode = await transformCharacterInCharacterNodeDto(characters, familyName);

                        CharacterNodeDto character = charactersNode.FirstOrDefault(node => node.name == characterName);

                        await SetLevelToAllDescendantInStack(character, charactersNode, familyName);
                    }
                }
            }

            if ( (await _characterRepositoryGraph.DeleteCharacter(characterName)) &&
                (await _characterRepositoryMongo.DeleteCharacterById(characterId)) && 
                (await _universeRepositoryMongo.RemoveCharacterFromUniverse(universeId, characterId)))
            { 
                return true; 
            } 
            return false;
        }

        public async Task<bool> SetLevelCharacter(string characterName, string familyTreeName, int level)
        {
            return await _characterRepositoryGraph.SetLevelFamilyFrom(characterName, familyTreeName, level);
        }
    }
}
