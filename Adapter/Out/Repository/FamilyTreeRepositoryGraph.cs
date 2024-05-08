using UniverseCreation.API.Adapter.Out.DataAccess;

namespace UniverseCreation.API.Adapter.Out.Repository
{
   /* public interface IFamilyTreeRepositoryGraph
    {
        Task<List<Dictionary<string, object>>> MatchAllFamilyTreeByUniverseName(string universeName);
        Task<bool> AddNewFamilyTree(string universeName, string familyTreeName);
        Task<List<Dictionary<string, object>>> FindFamilyTree(string universeName);
    }*/
    public class FamilyTreeRepositoryGraph 
    {
        private INeo4jDataAccess _neo4JDataAccess;
        private ILogger<FamilyTreeRepositoryGraph> _logger;

        public FamilyTreeRepositoryGraph(INeo4jDataAccess neo4JDataAccess, ILogger<FamilyTreeRepositoryGraph> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        // get all familyTree from an universe
        public async Task<List<Dictionary<string, object>>> MatchFamiliesTreesFromUniverse(string universe)
        {
            var query = @"MATCH(univers: Universe { name: $universe })< - [:BELONGS_TO] - (familyTree: FamilyTree) 
                        RETURN familyTree";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "universe", universe } };

            var familieTree = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "familyTree", parameters);

            return familieTree;
        }

        // Creation of a new familytree
        public async Task<bool> CreateFamilyTree(string universe, string familyTreeName)
        {
            if (familyTreeName != null && !string.IsNullOrWhiteSpace(familyTreeName))
            {
                var query = @"MATCH (universe: Universe {name: $universe })
                            CREATE (familyTree: FamilyTree {name: $familyTreeName }),
	                        (familyTree)-[:BELONGS_TO]->(universe)";

                IDictionary<string, object> parameters = new Dictionary<string, object> { 
                    { "universe", universe }, 
                    { "familyTreeName", familyTreeName } 
                };

                _logger.LogInformation($"Family tree '{familyTreeName}' created successfully for universe '{universe}'");

                return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query, parameters);
            }
            else
            {
                throw new System.ArgumentNullException(nameof(familyTreeName), "FamilyTreeName must not be null");
            }
        }

        // find the family tree from his name
        public async Task<List<Dictionary<string, object>>> FindFamilyTree(string familyTreeName)
        {
            var query = @"MATCH (familyTree: FamilyTree {name: $familyTreeName})
                          RETURN familyTree";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "familyTreeName", familyTreeName } };

            var universe = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "familyTree", parameters);

            if (universe != null && universe.Count == 0)
            {
                universe = null;
            }

            return universe;
        }
    }
}
