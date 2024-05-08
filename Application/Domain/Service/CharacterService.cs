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

        public async Task<List<Dictionary<string, object>>> FindAllCharactersFromUniverseName(string universe)
        {
            var characters = await _characterPersistance.GetAllCharactersFromUniverseName(universe);
            return characters;
        }

        public async Task<List<CharacterNodeDto>> FindAllCharactersFromFamilyTree(string family_treeName)
        {
            /*var characters = await _characterPersistance.GetAllCharactersFromFamilyTree(family_treeName);
            return characters;*/

            var characters = await _characterPersistance.GetAllCharactersFromFamilyTree(family_treeName);
            var characterNodeDtos = new List<CharacterNodeDto>();

            foreach (var character in characters)
            {
                var characterName = character["name"].ToString();

                var childrenDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Enfant");
                var parentsDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Parent");
                var marriedDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Marrié");
                var divorcedDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "Divorcé");
                var coupleDict = await _characterPersistance.GetAllRelationForCharacter(characterName, "En couple");
                var levelDict = await _characterPersistance.GetLevelFamilyTreeForCharacter(characterName, family_treeName);

                var children = childrenDict.Select(child => child["name"].ToString()).ToList();
                var parents = parentsDict.Select(parent => parent["name"].ToString()).ToList();
                var married = marriedDict.Select(marriage => marriage["name"].ToString()).ToList();
                var divorced = marriedDict.Select(divorce => divorce["name"].ToString()).ToList();
                var couple = marriedDict.Select(couple => couple["name"].ToString()).ToList();
                var level = levelDict.Select(level => Convert.ToInt32(level["level"])).FirstOrDefault();

                var characterNodeDto = new CharacterNodeDto
                {
                    name = characterName,
                    children = children,
                    parents = parents,
                    married = married,
                    divorced = divorced,
                    couple = couple,
                    level = level,
                };

                characterNodeDtos.Add(characterNodeDto);
            }


            return characterNodeDtos;
        }

        public async Task<bool> InsertCharacterToFamilyTree(string familyTreeName, string characterName)
        {
            return await _characterPersistance.ConnectCharacterToFamilyTree(familyTreeName, characterName);
        }

        public async Task<bool> RemoveCharacterToFamilyTree(string familyTreeName, string characterName)
        {
            return await _characterPersistance.DisconnectCharacterToFamilyTree(familyTreeName, characterName);
        }

        public async Task<bool> InsertRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription)
        {
            return await _characterPersistance.ConnectTwoCharacters(characterName1, characterName2, relationDescription);
        }

        public async Task<bool> RemoveRelationBetweenCharacters(string characterName1, string characterName2)
        {
            return await _characterPersistance.DisconnectTwoCharacters(characterName1, characterName2);
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
    }
}
