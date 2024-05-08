using UniverseCreation.API.Adapter.Out.Persistance;
using UniverseCreation.API.Application.Port.In;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Application.Domain.Service
{
    public class FamilyTreeService : IFamilyTreeService
    {
        private readonly IFamilyTreePersistance _familyTreePersistance;
        public FamilyTreeService(IFamilyTreePersistance familyTreePersistance)
        {
            this._familyTreePersistance = familyTreePersistance;
        }

        public async Task<List<Dictionary<string, object>>> FindAllFamiliesTreesFromUniverse(string universeName)
        {
            var families_trees = await _familyTreePersistance.GetAllFamiliesTreesFromUniverse(universeName);
            return families_trees;
        }

        public async Task<bool> AddFamilyTree(string familyTreeName, string characterName)
        {
            return await _familyTreePersistance.InsertNewFamilyTree(familyTreeName, characterName);
        }
    }
}
