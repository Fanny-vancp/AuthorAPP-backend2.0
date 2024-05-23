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

        /*
        [HttpPost]
        public ActionResult<UniverseDto> CreationUniverse([FromBody] UniverseForCreationDto universe)
        {
            try
            {
                // to be improved
                var maxUniverse = UniversesDataStores.Current.Universes.Max(p => p.Id);

                var finalUniverse = new UniverseDto()
                {
                    Id = ++maxUniverse,
                    Title = universe.Title,
                    LiteraryGenre = universe.LiteraryGenre
                };

                UniversesDataStores.Current.Universes.Add(finalUniverse);

                return CreatedAtRoute("GetUniverse",
                    new
                    {
                        idUnivers = finalUniverse.Id
                    },
                    finalUniverse);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception occurred while creating universe: {0}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPatch("{universeId}")]
        public ActionResult PartiallyUdpateCharacter(int universeId,
            [FromBody] JsonPatchDocument<UniverseForUpdateDto> patchDocument)
        {
            try
            {
                // find universe
                var universeFromStore = UniversesDataStores.Current.Universes.FirstOrDefault(c => c.Id == universeId);

                if (universeFromStore == null)
                {
                    _logger.LogInformation($"Universe with id {universeId} was'nt found when updating universe.");
                    return NotFound();
                }

                var universeToPatch =
                    new UniverseForUpdateDto()
                    {
                        Title = universeFromStore.Title,
                        LiteraryGenre = universeFromStore.LiteraryGenre,
                    };

                patchDocument.ApplyTo(universeToPatch, ModelState);

                // check the enter properties
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // check the enter properties after a update
                if (!TryValidateModel(universeToPatch))
                {
                    return BadRequest(ModelState);
                }

                // replace de value
                universeFromStore.Title = universeToPatch.Title;
                universeFromStore.LiteraryGenre = universeToPatch.LiteraryGenre;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while updating universe with id {universeId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }

        [HttpDelete("{universeId}")]
        public ActionResult DeleteCharacter(int universeId)
        {
            try
            {
                // find universe
                var universeFromStore = UniversesDataStores.Current.Universes.FirstOrDefault(c => c.Id == universeId);

                if (universeFromStore == null)
                {
                    _logger.LogInformation($"Universe with id {universeId} was'nt found when deleting universe.");
                    return NotFound();
                }

                // delete permanently 
                UniversesDataStores.Current.Universes.Remove(universeFromStore);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while deleting universe with id {universeId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }*/
    }
}
