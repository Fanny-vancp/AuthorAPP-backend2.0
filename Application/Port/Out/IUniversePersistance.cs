using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Application.Port.Out
{
    public interface IUniversePersistance
    {
        Task<List<UniverseDto>> GetAllUniverses();
        Task<UniverseDetailsDto> GetUniverseById(string id);
        Task<bool> AddNewUniverse(UniverseForCreationDto universe);
    }
}
