namespace UniverseCreation.API.Application.Port.Out
{
    public interface IFamilyTreePersistance
    {
        Task<List<Dictionary<string, object>>> GetAllFamiliesTreesFromUniverse(string universeName);
        Task<bool> InsertNewFamilyTree(string universeName, string familyTreeName);
        //Task<List<Dictionary<string, object>>> FindFamilyTree(string universeName);
    }
}
