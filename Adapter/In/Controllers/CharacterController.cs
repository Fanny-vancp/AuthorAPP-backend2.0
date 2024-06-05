using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.DataStore.ModelDto;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.In;
using Swashbuckle.AspNetCore.Annotations;

namespace UniverseCreation.API.Adapter.In.Controllers
{
    [ApiController]
    [Route("api/universes/{universe}/characters")]
    public class CharactersController : ControllerBase
    {
        private readonly ILogger<CharactersController> _logger;
        //private readonly ICharacterRepositoryGraph _characterRepositoryGraph;
        private readonly ICharacterService _characterService;

        public CharactersController(ILogger<CharactersController> logger, ICharacterService characterService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _characterService = characterService;
        }

        
        [HttpGet("{characterName}")]
        //[SwaggerOperation(Summary = "Search character by name", Description = "Retrieve a character by their name.")]
        //[SwaggerResponse(200, "The character was found.", typeof(CharacterDto))]
        //[SwaggerResponse(404, "Character not found.")]
        //[SwaggerResponse(500, "An error occurred while processing the request.")]
        public async Task<IActionResult> SearchCharacterByNameNode(string universe, string characterName)
        {
            try
            {
                var characters = await _characterService.FindCharacterFromString(universe, characterName);

                return Ok(characters);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters for universe with the name : {universe}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        
        [HttpGet()]
        public async Task<IActionResult> GetAllCharactersByUniverseNameNode(string universe)
        {
            try
            {
                var characters = await _characterService.FindAllCharactersNodeFromUniverseName(universe);

                return Ok(characters);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters for universe with the name : {universe}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCharacter(string universe,
            [FromBody] CharacterForCreationDto character)
        {
            try
            {
                await _characterService.CreateNewCharacter(universe, character);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while creating a new character with the name {character.Pseudo} in the universe with the id {universe}",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpDelete("{characterId}")]
        public async Task<IActionResult> DeleteCharacter(string universe, string characterId) 
        {
            try
            {
                await _characterService.DeleteCharacter(universe, characterId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while deleting a character with the id {characterId} in the universe with the id {universe}",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }


        [HttpGet("/api/families_trees/{familyTreeName}/characters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CharacterNodeDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCharactersByFamilyTreeNameNode(string familyTreeName)
        {
            try
            {
                var characters = await _characterService.FindAllCharactersFromFamilyTree(familyTreeName);
                return Ok(characters);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters for the family name : {familyTreeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }


        [HttpPost("/api/families_trees/{familyTreeName}/characters")]
        public async Task<IActionResult> AddCharacterInFamilyTree([FromBody] RelationForCreationDto relationForCreationDto)
        {
            try
            {
                await _characterService.InsertCharacterToFamilyTree(relationForCreationDto.EndPoint, relationForCreationDto.StratPoint);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while adding characters for the family name name : {relationForCreationDto.StratPoint}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpDelete("/api/families_trees/{familyTreeName}/characters/{characterName}")]
        public async Task<IActionResult> DeleteCharacterFromFamilyTree(string familyTreeName, string characterName)
        {
            try
            {
                await _characterService.RemoveCharacterToFamilyTree(familyTreeName, characterName);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while deleting characters from the family name : {familyTreeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPost("/api/families_trees/{familyTreeName}/characters/relation")]
        public async Task<IActionResult> AddRelationBetweenCharacters(string familyTreeName, 
            [FromBody] RelationForCreationDto relationForCreationDto)
        {
            try
            {
                await _characterService.InsertRelationBetweenCharacters(relationForCreationDto.StratPoint, 
                    relationForCreationDto.EndPoint, relationForCreationDto.descriptionRelation, familyTreeName);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while creating relation between two characters from the family name : {familyTreeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpDelete("/api/families_trees/{familyTreeName}/characters/{characterName1}/relation/{characterName2}")]
        public async Task<IActionResult> DeleteRelationBetweenCharacter(string characterName1, string characterName2, string familyTreeName)
        {
            try
            {
                await _characterService.RemoveRelationBetweenCharacters(characterName1, characterName2, familyTreeName);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while removing relation between two characters from the family name : {familyTreeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPatch("/api/families_trees/{familyTreeName}/characters/relation")]
        public async Task<IActionResult> PatchRelationBetweenCharacters(string familyTreeName, [FromBody] RelationForUpdatingDto relationForUpdatingDto)
        {
            try
            {
                await _characterService.UpdateRelationBetweenCharacters(relationForUpdatingDto.StratPoint, relationForUpdatingDto.EndPoint, relationForUpdatingDto.descriptionRelation);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while updating relation between two characters from the family name : {familyTreeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpGet("/api/families_trees/{familyTreeName}/characters/{character_name}/relation/{relation_description}")]
        public async Task<IActionResult> GetRelationForCharacter(string familyTreeName, string character_name, string relation_description)
        {
            try
            {
                var characters = await _characterService.FindAllRelationForCharacter(character_name, relation_description);
                return Ok(characters);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters with a relation {relation_description} for the character {character_name} for the family name name : {familyTreeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }


        [HttpGet("/api/universe/{universeId}/characters/details/")]
        public async Task<IActionResult> GetAllCharacters(String universeId)
        {
            try
            {
                var characters = await _characterService.FindAllCharacters(universeId);
                return Ok(characters);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting all characters from the universe with the id: {universeId}",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpGet("/api/universe/characters/details/{characterId}")]
        public async Task<IActionResult> GetCharaterById(String characterId)
        {
            try
            {
                var characters = await _characterService.FindCharacterById(characterId);
                return Ok(characters);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting character with the id: {characterId}",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPatch("/api/characters/{characterName}")]
        public async Task<IActionResult> PatchCharacter(string characterName, [FromBody] CharacterDetailsDto characterDetails)
        {
            try
            {
                await _characterService.ChangeCharacter(characterDetails, characterName);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while updating character with the name {characterName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }
    }
}