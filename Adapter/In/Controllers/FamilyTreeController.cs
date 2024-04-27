using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Adapter.In.Controllers
{
    [ApiController]
    [Route("api/universes/{universe}/families_trees")]
    public class FamilyTreeController : ControllerBase
    {
        private readonly ILogger<FamilyTreeController> _logger;
        private readonly IFamilyTreeRepositoryGraph _familyTreeRepositoryGraph;
        private readonly IUniverseRepositoryGraph _universeRepositoryGraph;

        public FamilyTreeController(ILogger<FamilyTreeController> logger, IFamilyTreeRepositoryGraph familyTreeRepositoryGraph, IUniverseRepositoryGraph universeRepositoryGraph)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _familyTreeRepositoryGraph = familyTreeRepositoryGraph;
            _universeRepositoryGraph = universeRepositoryGraph;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllFamilyTreeFromUniverse(string universe)
        {
            try
            {
                // check if the universe exist
                var universeFinded = await _universeRepositoryGraph.FindUniverse(universe);
                if (universeFinded == null)
                {
                    _logger.LogInformation($"Universe with the name {universe} was'nt found when accessing familyTree.");
                    return NotFound();
                }

                var familyTree = await _familyTreeRepositoryGraph.MatchAllFamilyTreeByUniverseName(universe);

                return Ok(familyTree);
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
        public async Task<IActionResult> CreateFamilyTree(string universe, [FromBody] FamilyTreeForCreationDto familyTree)
        {
            try
            {

                // check if the universe exist
                var universeFinded = await _universeRepositoryGraph.FindUniverse(universe);
                if (universeFinded == null)
                {
                    _logger.LogInformation($"Universe with the name {universe} was'nt found when creating familyTree.");
                    return NotFound();
                }

                await _familyTreeRepositoryGraph.AddNewFamilyTree(universe, familyTree.Name);

                return Ok();
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
