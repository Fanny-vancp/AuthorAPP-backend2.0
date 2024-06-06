using Microsoft.AspNetCore.Mvc;
using UniverseCreation.API.Application.Domain.Service;
using UniverseCreation.API.Application.Port.In;

namespace UniverseCreation.API.Adapter.In.Controllers
{
    [ApiController]
    [Route("api/universes/{universe}/books")]
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
        public async Task<IActionResult> GetAllBookByUniverseId(string universe)
        {
            try
            {
                var books = await _bookService.FindAllBooks(universe);

                return Ok(books);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting books for universe with the name : {universe}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }
    }
}
