using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Adapter.Out.Persistance
{
    public class UniversePersistance : IUniversePersistance
    {
        private readonly ILogger<UniversePersistance> _logger;
        private readonly UniverseRepositoryMongo _universeRepositoryMongo;
        private readonly UniverseRepositoryGraph _universeRepositoryGraph;

        public UniversePersistance(ILogger<UniversePersistance> logger, UniverseRepositoryMongo universeRepositoryMongo,
            UniverseRepositoryGraph universeRepositoryGraph)
        {
            _logger = logger;
            _universeRepositoryMongo = universeRepositoryMongo;
            _universeRepositoryGraph = universeRepositoryGraph;
        }

        public async Task<List<UniverseDto>> GetAllUniverses()
        {
            var universes = await _universeRepositoryMongo.CatchAllUniverse();

            /*var universes = universesDetails.Select(u => new UniverseDto
            {
                Id = u.Id,
                Name = u.Name,
                LiteraryGenre = u.LiteraryGenre
            }).ToList();*/

            return universes;
        }
        public async Task<UniverseDetailsDto> GetUniverseById(string id)
        {
            var universe = await _universeRepositoryMongo.CatchUniverseById(id);
            return universe;
        }

        public async Task<bool> AddNewUniverse(UniverseForCreationDto universe)
        {
            await _universeRepositoryGraph.CreateUniverseNode(universe);
            return await _universeRepositoryMongo.InsertUniverse(universe);
        }

    }
}
