using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.Out;

namespace UniverseCreation.API.Adapter.Out.Persistance
{
    public class BookPersistance : IBookPersistance
    {
        private readonly ILogger<BookPersistance> _logger;
        private readonly BookRepositoryMongoDB _bookRepositoryMongo;
        private readonly UniverseRepositoryMongo _universeRepositoryMongo;

        public BookPersistance(ILogger<BookPersistance> logger, 
            BookRepositoryMongoDB bookRepositoryMongo, 
            UniverseRepositoryMongo universeRepository)
        {
            _logger = logger;
            _bookRepositoryMongo = bookRepositoryMongo;
            _universeRepositoryMongo = universeRepository;
        }

        public async Task<List<BookDto>> GetAllBooks(string idUniverse)
        {
            var universe = await _universeRepositoryMongo.CatchUniverseById(idUniverse);
            var allBooks = new List<BookDto>();

            if (universe != null && universe.Books != null)
            {
                foreach (var bookReference in universe.Books)
                {
                    var book = await _bookRepositoryMongo.CatchBookById(bookReference.BookId);
                    if (book != null)
                    {
                        allBooks.Add(book);
                    }
                }
            }

            return allBooks;
        }
    }
}
