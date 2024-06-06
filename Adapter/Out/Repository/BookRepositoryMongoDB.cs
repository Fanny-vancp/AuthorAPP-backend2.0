using MongoDB.Bson;
using MongoDB.Driver;
using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Adapter.Out.Repository
{
    public class BookRepositoryMongoDB
    {
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly ILogger<BookRepositoryMongoDB> _logger;

        public BookRepositoryMongoDB(IConfiguration configuration, ILogger<BookRepositoryMongoDB> logger)
        {
            _logger = logger;
            try
            {
                var mongoDbConnection = configuration["ApplicationSettings:MongoDBConnection"];
                var databaseName = configuration["ApplicationSettings:MongoDBDatabase"];
                var collectionName = configuration["ApplicationSettings:BookCollectionName"];

                _logger.LogInformation("Connecting to MongoDB with connection string: {ConnectionString}", mongoDbConnection);
                var client = new MongoClient(mongoDbConnection);
                var database = client.GetDatabase(databaseName);
                _collection = database.GetCollection<BsonDocument>(collectionName);
                _logger.LogInformation("Successfully connected to MongoDB and obtained the collection.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MongoDB.");
                throw;
            }
        }

        public async Task<List<BookDto>> CatchAllBooks()
        {
            try
            {
                _logger.LogInformation("Fetching all books from MongoDB with selected properties.");

                var projection = Builders<BsonDocument>.Projection
                    .Include("_id")
                    .Include("title");

                var documents = await _collection
                    .Find(_ => true)
                    .Project(projection)
                    .ToListAsync();

                var bookList = new List<BookDto>();

                if (documents == null || !documents.Any())
                {
                    _logger.LogWarning("No documents found in the book collection.");
                    return bookList;
                }

                foreach (var document in documents)
                {
                    bookList.Add(ConvertBsonToBook(document));
                }

                _logger.LogInformation("Successfully fetched {Count} book.", bookList.Count);
                return bookList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all books.");
                throw;
            }
        }

        public async Task<BookDto> CatchBookById(string bookId)
        {
            try
            {
                _logger.LogInformation("Fetching book by ID: {bookId}", bookId);
                var universeCursor = await _collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(bookId)));
                var document = universeCursor.FirstOrDefault();
                var book = ConvertBsonToBook(document);

                if (book == null)
                {
                    _logger.LogWarning("No universe found with ID: {bookId}", bookId);
                    return new BookDto();
                }

                _logger.LogInformation("Successfully fetched book with ID: {bookId}", bookId);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching book by ID.");
                throw;
            }
        }

        public async Task<BookDetailsDto> CatchBookDetailsById(string bookId)
        {
            try
            {
                _logger.LogInformation("Fetching book by ID: {bookId}", bookId);
                var bookCursor = await _collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(bookId)));
                var document = bookCursor.FirstOrDefault();
                var book = ConvertBsonToBookDetails(document);

                if (book == null)
                {
                    _logger.LogWarning("No book found with ID: {bookId}", bookId);
                    return new BookDetailsDto();
                }

                _logger.LogInformation("Successfully fetched character with ID: {bookId}", bookId);
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching book by ID.");
                throw;
            }
        }

        public async Task<String> InsertBook(BookForCreationDto book)
        {
            try
            {
                _logger.LogInformation("Creating a new book.");
                BsonArray chapter = new BsonArray();
                BsonArray scene = new BsonArray();


                var document = new BsonDocument
                {
                    { "title", book.Title },
                    { "chapter", chapter },
                    { "scene", scene }
                };

                await _collection.InsertOneAsync(document);

                var bookId = document["_id"].AsObjectId.ToString();
                _logger.LogInformation("Successfully created book with ID: {bookId}", bookId);

                return bookId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a book.");
                throw;
            }
        }

        public async Task<bool> UpdateBook(BookDetailsDto book)
        {
            try
            {
                _logger.LogInformation("Updating book with ID: {BookId}", book.Id);

                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(book.Id));

                var update = Builders<BsonDocument>.Update
                    .Set("title", book.Title)
                    .Set("chapter", new BsonArray(book.Chapters.Select(chapter =>
                        new BsonDocument
                        {
                    { "index", chapter.Index },
                    { "value", chapter.Value }
                        })))
                    .Set("scene", new BsonArray(book.Scenes.Select(scene =>
                        new BsonDocument
                        {
                    { "index", scene.Index },
                    { "value", scene.Value }
                        })));

                var result = await _collection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    _logger.LogWarning("No book found with ID: {BookId} to update.", book.Id);
                    return false;
                }

                _logger.LogInformation("Successfully updated book with ID: {BookId}", book.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating book with ID: {BookId}", book.Id);
                throw;
            }
        }

        public async Task<bool> DeleteBookById(string bookId)
        {
            try
            {
                _logger.LogInformation("Deleting book with ID: {BookId}", bookId);

                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(bookId));

                var result = await _collection.DeleteOneAsync(filter);

                if (result.DeletedCount == 0)
                {
                    _logger.LogWarning("No book found with ID: {BookId} to delete.", bookId);
                    return false;
                }

                _logger.LogInformation("Successfully deleted book with ID: {BookId}", bookId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting book with ID: {BookId}", bookId);
                throw;
            }
        }

        private BookDto ConvertBsonToBook(BsonDocument document)
        {
            if (document == null) return null;


            return new BookDto
            {
                Id = document["_id"].AsObjectId.ToString(),
                Title = document["title"].AsString
            };
        }

        private BookDetailsDto ConvertBsonToBookDetails(BsonDocument document)
        {
            if (document == null) return null;


            return new BookDetailsDto
            {
                Id = document["_id"].AsObjectId.ToString(),
                Title = document["title"].AsString,
                Chapters = document["chapter"].AsBsonArray.Select(chapter => new Chapter
                {
                    Index = (int)chapter["index"],
                    Value = chapter["value"].AsString
                }).ToList(),
                Scenes = document["scene"].AsBsonArray.Select(scene => new Scene
                {
                    Index = (int)scene["index"],
                    Value = scene["value"].AsString
                }).ToList()
            };
        }
    }
}
