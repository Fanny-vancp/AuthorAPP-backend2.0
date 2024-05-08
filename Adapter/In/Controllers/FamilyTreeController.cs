using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.In;

namespace UniverseCreation.API.Adapter.In.Controllers
{
    [ApiController]
    [Route("api/universes/{universe}/families_trees")]
    public class FamilyTreeController : ControllerBase
    {
        private readonly ILogger<FamilyTreeController> _logger;
        private readonly IFamilyTreeService _familyTreeService;

        public FamilyTreeController(ILogger<FamilyTreeController> logger, IFamilyTreeService familyTreeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _familyTreeService = familyTreeService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllFamiliesTreesByUniverseName(string universe)
        {
            try
            {
                var familyTree = await _familyTreeService.FindAllFamiliesTreesFromUniverse(universe);

                return Ok(familyTree);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting families tree for universe with the name : {universe}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPost()]
        public async Task<IActionResult> PostNewFamilyTree(string universe, [FromBody] FamilyTreeForCreationDto familyTree)
        {
            try
            {
                await _familyTreeService.AddFamilyTree(universe, familyTree.Name);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while creating family tree for universe with the name : {universe}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }
    }
}
