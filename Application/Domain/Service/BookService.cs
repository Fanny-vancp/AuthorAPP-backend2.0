using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.In;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Application.Domain.Service
{
    public class BookService : IBookService
    {
        private readonly IBookPersistance _bookPersistance;
        public BookService(IBookPersistance bookPersistance)
        {
            this._bookPersistance = bookPersistance;
        }
        public async Task<List<BookDto>> FindAllBooks(string idUniverse)
        {
            return await _bookPersistance.GetAllBooks(idUniverse);
        }

        public async Task<BookDetailsDto> FindBookById(string idBook)
        {
            return await _bookPersistance.GetBookById(idBook);
        }

        public async Task<bool> CreateNewBook(string idUniverse, BookForCreationDto book)
        {
            return await _bookPersistance.AddNewBook(idUniverse, book);
        }

        public async Task<bool> DeleteBook(string idUniverse, string idBook)
        {
            return await _bookPersistance.RemoveBook(idUniverse, idBook);
        }

        public async Task<bool> ChangeBook(BookDetailsDto book)
        {
            return await _bookPersistance.ReformBook(book);
        }
    }
}
