namespace UniverseCreation.API.Application.Port.Out
{
    public interface ICharacterPersistance
    {
        Task<List<Dictionary<string, object>>> GetAllCharactersFromUniverseName(string universeName);
        Task<List<Dictionary<string, object>>> GetAllCharactersFromFamilyTree(string familyTree);
        Task<List<Dictionary<string, object>>> GetCharacterFromString(string searchName, string universeName);
        Task<bool> ConnectCharacterToFamilyTree(string familyTreeName, string characterName);
        Task<bool> DisconnectCharacterToFamilyTree(string familyTreeName, string characterName);
        Task<bool> ConnectTwoCharacters(string characterName1, string characterName2, string relationDescription);
        Task<bool> DisconnectTwoCharacters(string characterName1, string  characterName2);
        Task<bool> FixRelationBetweenCharacters(string characterName1, string characterName2, string relationDescription);
        Task<List<Dictionary<string, object>>> GetAllRelationForCharacter(string characterName, string relationDescription);
        Task<List<Dictionary<string, object>>> GetLevelFamilyTreeForCharacter(string characterName, string familyTree);

        
    }
}
