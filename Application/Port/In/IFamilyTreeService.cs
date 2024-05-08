namespace UniverseCreation.API.Application.Port.In
{
    public interface IFamilyTreeService
    {
        Task<List<Dictionary<string, object>>> FindAllFamiliesTreesFromUniverse(string universeName);
        Task<bool> AddFamilyTree(string universeName, string familyTreeName);
    }
}
