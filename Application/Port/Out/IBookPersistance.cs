using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Application.Port.Out
{
    public interface IBookPersistance
    {
        Task<List<BookDto>> GetAllBooks(string idUniverse);
    }
}
