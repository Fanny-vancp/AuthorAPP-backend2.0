using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Application.Port.In
{
    public interface IBookService
    {
        Task<List<BookDto>> FindAllBooks(string idUniverse);
        Task<BookDetailsDto> FindBookById(string idBook);
        Task<bool> CreateNewBook(string idUniverse, BookForCreationDto book);
        Task<bool> DeleteBook(string univereseId, string bookId);
        Task<bool> ChangeBook(BookDetailsDto book);
    }
}
