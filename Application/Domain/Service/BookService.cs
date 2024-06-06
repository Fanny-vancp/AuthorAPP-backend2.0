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
    }
}
