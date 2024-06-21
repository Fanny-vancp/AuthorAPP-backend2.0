using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Application.Port.Out
{
    public interface IBookPersistance
    {
        Task<List<BookDto>> GetAllBooks(string idUniverse);
        Task<BookDetailsDto> GetBookById(string idBook);
        Task<bool> AddNewBook(string idUniverse, BookForCreationDto book);
        Task<bool> RemoveBook(string idUniverse, string idBook);
        Task<bool> ReformBook(BookDetailsDto book);
    }
}
