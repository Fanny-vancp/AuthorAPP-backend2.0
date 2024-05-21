using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Application.Port.In
{
    public interface IUniverseService
    {
        Task<List<UniverseDto>> FindAllUniverse();
        Task<UniverseDto> FindUniverseById(string id);
    }
}
