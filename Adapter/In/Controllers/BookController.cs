using Microsoft.AspNetCore.Mvc;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Domain.Service;
using UniverseCreation.API.Application.Port.In;

namespace UniverseCreation.API.Adapter.In.Controllers
{
    [ApiController]
    [Route("api/universes/{universeId}/books")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookService _bookService;

        public BooksController(ILogger<BooksController> logger, IBookService bookService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bookService = bookService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllBookByUniverseId(string universeId)
        {
            try
            {
                var books = await _bookService.FindAllBooks(universeId);

                return Ok(books);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting books for universe with the name : {universeId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBookId(string universeId, string bookId)
        {
            try
            {
                var book = await _bookService.FindBookById(bookId);

                return Ok(book);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting the book withe the id : {bookId} for universe with the id : {universeId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPost()]
        public async Task<IActionResult> PostNewBook(string universeId, [FromBody] BookForCreationDto book)
        {
            try
            {
                await _bookService.CreateNewBook(universeId, book);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while create a new book for universe with the id : {universeId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteCharacter(string universeId, string bookId)
        {
            try
            {
                await _bookService.DeleteBook(universeId, bookId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while deleting a book with the id {bookId} in the universe with the id {universeId}",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPatch()]
        public async Task<IActionResult> PatchBook(string universeId, [FromBody] BookDetailsDto book)
        {
            try
            {
                await _bookService.ChangeBook(book);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while updating relation book with the id : {book.Id} from the universe : {universeId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }
    }
}
