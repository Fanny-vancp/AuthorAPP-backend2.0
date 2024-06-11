using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Application.Port.Out
{
    public interface ICharacterPersistance
    {
        Task<List<Dictionary<string, object>>> GetAllCharactersNodeFromUniverseName(string universeName);
        Task<List<Dictionary<string, object>>> GetAllCharactersFromFamilyTree(string familyTree);
        Task<List<Dictionary<string, object>>> GetCharacterFromString(string searchName, string universeName);
        Task<bool> ConnectCharacterToFamilyTree(string familyTreeName, string characterName, List<CharacterNodeDto> charactersfamilyMembers);
        Task<bool> DisconnectCharacterToFamilyTree(string familyTreeName, string characterName);
        Task<bool> ConnectTwoCharacters(string characterName1, string characterName2, string relationDescription, List<CharacterNodeDto> characters, string familyTreeName);
        Task<bool> DisconnectTwoCharacters(string characterName1, string  characterName2, string familyTreeName, List<CharacterNodeDto> characters);
        Task<bool> FixRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription);
        Task<List<Dictionary<string, object>>> GetAllRelationForCharacter(string characterName, string relationDescription);
        Task<List<Dictionary<string, object>>> GetLevelFamilyTreeForCharacter(string characterName, string familyTree);

        Task<List<CharacterDto>> GetAllCharacters(string universeId);
        Task<CharacterDetailsDto> GetCharacterById(string characterId);
        Task<bool> AddNewCharacter(string idUniverse, CharacterForCreationDto character);
        Task<bool> ReformCharacter(CharacterDetailsDto character, string characterName);
        Task<bool> RemoveCharacter( string universeId, string characterId, string characterName);
        Task<bool> SetLevelCharacter(string characterName, string familyTreeName, int level);
    }
}
