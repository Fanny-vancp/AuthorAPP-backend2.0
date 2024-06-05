using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
//using UniverseCreation.API.DataStore.ModelDto;
using UniverseCreation.API.DataStore;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.In;

namespace UniverseCreation.API.Adapter.In.Controllers
{
    [ApiController]
    [Route("api/universes")]
    public class UniversesController : ControllerBase
    {
        private readonly ILogger<UniversesController> _logger;
        private readonly IUniverseService _universeService;

        public UniversesController(ILogger<UniversesController> logger, IUniverseService universeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _universeService = universeService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetUniverses()
        {
            try
            {
                var universes = await _universeService.FindAllUniverse();

                return Ok(universes);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception occurred while getting universes: {0}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpGet("{idUniverse}", Name = "GetUniverse")]
        public async Task<IActionResult> GetUniverse(string idUniverse)
        {

            try
            {
                var universe = await _universeService.FindUniverseById(idUniverse);

                return Ok(universe);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception occurred while getting universes: {0}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> PostNewUniverse([FromBody]UniverseForCreationDto universe)
        {
            try
            {
                await _universeService.CreateNewUniverse(universe);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception occurred while getting universes: {0}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }
    }
}
