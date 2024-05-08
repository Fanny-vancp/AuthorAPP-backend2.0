using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Adapter.Out.Persistance
{
    public class FamilyTreePersistance : IFamilyTreePersistance
    {
        private readonly ILogger<FamilyTreePersistance> _logger;
        private readonly FamilyTreeRepositoryGraph _familyTreeRepositoryGraph;
        private readonly UniverseRepositoryGraph _universeRepositoryGraph;

        public FamilyTreePersistance(ILogger<FamilyTreePersistance> logger, FamilyTreeRepositoryGraph familyTreeRepositoryGraph, UniverseRepositoryGraph universeRepositoryGraph)
        {
            _logger = logger;
            _familyTreeRepositoryGraph = familyTreeRepositoryGraph;
            _universeRepositoryGraph = universeRepositoryGraph;
        }

        public async Task<List<Dictionary<string, object>>> GetAllFamiliesTreesFromUniverse(string universeName)
        {
            // check if the universe exist
            var universeFinded = await _universeRepositoryGraph.FindUniverse(universeName);
            if (universeFinded == null)
            {
                _logger.LogInformation($"Universe with the name {universeName} was'nt found when accessing familyTree.");
                throw new InvalidOperationException($"Universe with the name {universeName} was'nt found when accessing familyTree.");
            }

            var families_trees = await _familyTreeRepositoryGraph.MatchFamiliesTreesFromUniverse(universeName);
            return families_trees;
        }
        public async Task<bool> InsertNewFamilyTree(string universeName, string familyTreeName)
        {
            // check if the universe exist
            var universeFound = await _universeRepositoryGraph.FindUniverse(universeName);
            if (universeFound == null)
            {
                _logger.LogInformation($"Universe with the name {universeName} was'nt found when adding family tree in universe.");
                throw new InvalidOperationException($"Universe with the name {universeName} wasn't found when adding family tree in universe.");
            }

            return await _familyTreeRepositoryGraph.CreateFamilyTree(universeName, familyTreeName);
        }
    }
}
