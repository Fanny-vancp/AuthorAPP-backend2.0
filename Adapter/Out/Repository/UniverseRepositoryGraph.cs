using UniverseCreation.API.Adapter.Out.DataAccess;

namespace UniverseCreation.API.Adapter.Out.Repository
{
    /*public interface IUniverseRepositoryGraph
    {
        Task<List<Dictionary<string, object>>> FindUniverse(string universeName);
    }*/
    public class UniverseRepositoryGraph 
    {
        private INeo4jDataAccess _neo4JDataAccess;
        private ILogger<UniverseRepositoryGraph> _logger;

        public UniverseRepositoryGraph(INeo4jDataAccess neo4JDataAccess, ILogger<UniverseRepositoryGraph> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        // find the universe from his name
        public async Task<List<Dictionary<string, object>>> FindUniverse(string universeName)
        {
            var query = @"MATCH (universe: Universe {name: $universeName})
                          RETURN universe";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "universeName", universeName } };

            var universe = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "universe", parameters);

            if (universe != null && universe.Count == 0)
            {
                universe = null;
            }

            return universe;
        }
    }
}
