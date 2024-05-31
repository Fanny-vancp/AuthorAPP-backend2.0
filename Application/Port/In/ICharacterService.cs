using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Application.Port.In
{
    public interface ICharacterService
    {
        Task<List<Dictionary<string, object>>> FindCharacterFromString(string searchName, string universeName);
        Task<List<Dictionary<string, object>>> FindAllCharactersNodeFromUniverseName(string universeName);
        Task<List<CharacterNodeDto>> FindAllCharactersFromFamilyTree(string family_treeName);
        Task<bool> InsertCharacterToFamilyTree(string familyTreeName, string characterName);
        Task<bool> RemoveCharacterToFamilyTree(string familyTreeName, string characterName);
        Task<bool> InsertRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription, string familyTreeName);
        Task<bool> RemoveRelationBetweenCharacters(string characterName1, string characterName2, string familyTreeName);
        Task<bool> UpdateRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription);
        Task<List<Dictionary<string, object>>> FindAllRelationForCharacter(string characterName, string relationDescription);

        Task<List<CharacterDto>> FindAllCharacters(string idUniverse);
        Task<CharacterDetailsDto> FindCharacterById(string idCharacter);
        Task<bool> CreateNewCharacter(string idUniverse,CharacterForCreationDto character);
    }
}
