using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Adapter.Out.Persistance
{
    public class UniversePersistance : IUniversePersistance
    {
        private readonly ILogger<UniversePersistance> _logger;
        private readonly UniverseRepositoryMongo _universeRepositoryMongo;

        public UniversePersistance(ILogger<UniversePersistance> logger, UniverseRepositoryMongo universeRepositoryMongo)
        {
            _logger = logger;
            _universeRepositoryMongo = universeRepositoryMongo;
        }

        public async Task<List<UniverseDto>> GetAllUniverses()
        {
            var universes = await _universeRepositoryMongo.CatchAllUniverse();
            return universes;
        }
        public async Task<UniverseDto> GetUniverseById(string id)
        {
            var universe = await _universeRepositoryMongo.CatchUniverseById(id);
            return universe;
        }

    }
}
