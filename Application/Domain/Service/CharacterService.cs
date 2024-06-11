using Microsoft.AspNetCore.Razor.TagHelpers;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.In;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Application.Domain.Service
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterPersistance _characterPersistance;
        public CharacterService(ICharacterPersistance characterPersistance) {
            this._characterPersistance = characterPersistance;
        }

        public async Task<List<Dictionary<string, object>>> FindCharacterFromString(string universe, string searchName)
        {
            var characters = await _characterPersistance.GetCharacterFromString(universe, searchName);
            return characters;
        }

        public async Task<List<Dictionary<string, object>>> FindAllCharactersNodeFromUniverseName(string universe)
        {
            var characters = await _characterPersistance.GetAllCharactersNodeFromUniverseName(universe);
            return characters;
        }

        public async Task<List<CharacterNodeDto>> FindAllCharactersFromFamilyTree(string family_treeName)
        {
            var characters = await _characterPersistance.GetAllCharactersFromFamilyTree(family_treeName);
            var characterNodeDtos = await transformCharacterInCharacterNodeDto(characters, family_treeName);

            if ((characterNodeDtos.Count == 1) && (characterNodeDtos[0].level == -1)) 
            {
                characterNodeDtos[0].level = 0;
                await _characterPersistance.SetLevelCharacter(characterNodeDtos[0].name, family_treeName, 0);
            }

            characterNodeDtos = characterNodeDtos.OrderBy(c => c.level).ToList();

            return characterNodeDtos;
        }

        private async Task<List<CharacterNodeDto>> transformCharacterInCharacterNodeDto(List<Dictionary<string, object>> characters, string family_treeName)
        {
            var characterNodeDtos = new List<CharacterNodeDto>();

            foreach (var character in characters)
            {
                var characterName = character["name"].ToString();

                var childrenDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Enfant");
                var parentsDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Parent");
                var marriedDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Marrié");
                var divorcedDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Divorcé");
                var coupleDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "En couple");
                var amantDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Amant");
                var levelDict = await _characterPersistance.GetLevelFamilyTreeForCharacter(characterName, family_treeName);

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

        public async Task<bool> InsertCharacterToFamilyTree(string familyTreeName, string characterName)
        {
            var characters = await _characterPersistance.GetAllCharactersFromFamilyTree(familyTreeName);
            var characterNodeDtos = await transformCharacterInCharacterNodeDto(characters, familyTreeName);
            return await _characterPersistance.ConnectCharacterToFamilyTree(familyTreeName, characterName, characterNodeDtos);
        }

        public async Task<bool> RemoveCharacterToFamilyTree(string familyTreeName, string characterName)
        {
            return await _characterPersistance.DisconnectCharacterToFamilyTree(familyTreeName, characterName);
        }

        public async Task<bool> InsertRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription, string familyTreeName)
        {
            var characters = await _characterPersistance.GetAllCharactersFromFamilyTree(familyTreeName);
            var characterNodeDtos = await transformCharacterInCharacterNodeDto(characters, familyTreeName);
            return await _characterPersistance.ConnectTwoCharacters(characterName1, characterName2, relationDescription, characterNodeDtos, familyTreeName);
        }

        public async Task<bool> RemoveRelationBetweenCharacters(string characterName1, string characterName2, string familyTreeName)
        {
            var characters = await _characterPersistance.GetAllCharactersFromFamilyTree(familyTreeName);
            var characterNodeDtos = await transformCharacterInCharacterNodeDto(characters, familyTreeName);
            return await _characterPersistance.DisconnectTwoCharacters(characterName1, characterName2, familyTreeName, characterNodeDtos);
        }

        public async Task<bool> UpdateRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription)
        {
            return await _characterPersistance.FixRelationBetweenCharacters(characterName1, characterName2, relationDescription);
        }

        public async Task<List<Dictionary<string, object>>> FindAllRelationForCharacter(string characterName, string relationDescription)
        {
            var characters = await _characterPersistance.GetAllRelationForCharacter(characterName, relationDescription);
            return characters;
        }

        public async Task<List<CharacterDto>> FindAllCharacters(string idUniverse)
        {
            var characters = await _characterPersistance.GetAllCharacters(idUniverse);
            return characters;
        }

        public async Task<CharacterDetailsDto> FindCharacterById(string idCharacter)
        {
            var character = await _characterPersistance.GetCharacterById(idCharacter);
            return character;
        }

        public async Task<bool> CreateNewCharacter(string idUniverse, CharacterForCreationDto character)
        {
            return await _characterPersistance.AddNewCharacter(idUniverse, character);
        }

        public async Task<bool> ChangeCharacter( CharacterDetailsDto character, string characterName)
        {
            return await _characterPersistance.ReformCharacter(character, characterName);
        }

        public async Task<bool> DeleteCharacter(string universeId, string characterId)
        {
            var character = await _characterPersistance.GetCharacterById(characterId);

            string characterName;
            if (character.FirstName == "")
            {
                characterName = character.Pseudo;
            } else
            {
                characterName = character.FirstName + " " + character.LastName;
            }
            return await _characterPersistance.RemoveCharacter( universeId, characterId, characterName);
        }
    }
}
