using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.In;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Application.Domain.Service
{
    public class UniverseService : IUniverseService
    {
        private readonly IUniversePersistance _universePersistance;
        public UniverseService(IUniversePersistance universePersistance)
        {
            this._universePersistance = universePersistance;
        }

        public async Task<List<UniverseDto>> FindAllUniverse()
        {
            return await _universePersistance.GetAllUniverses();
        }
        public async Task<UniverseDto> FindUniverseById(string id)
        {
            return await _universePersistance.GetUniverseById(id);
        }
    }
}
