using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.DataStore.ModelDto;
using UniverseCreation.API.Application.Domain.Model;
using UniverseCreation.API.Application.Port.In;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;

namespace UniverseCreation.API.Adapter.In.Controllers
{
    [ApiController]
    [Route("api/universes/{universe}/characters")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        private bool IsValidInput(string input)
        {
            // Accepts letters, numbers, spaces, periods, commas, hyphens, and apostrophes
            string pattern = @"^[a-zA-Z0-9\s\.\,\-\p{L}\'\’]+$";
            return Regex.IsMatch(input, pattern);
        }


        [HttpGet("{characterName}")]
        public async Task<IActionResult> SearchCharacterByNameNode(string universe, string characterName)
        {
            try
            {
                // Validation of user inputs
                if (string.IsNullOrEmpty(universe) || string.IsNullOrEmpty(characterName))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(universe) || !IsValidInput(characterName))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
        public async Task<IActionResult> GetAllCharactersByUniverseName(string universe)
        {
            try
            {
                // Validation of user inputs
                if (string.IsNullOrEmpty(universe))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(universe))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
                // Validation of user inputs
                if (string.IsNullOrEmpty(universe) || string.IsNullOrEmpty(character.UniverseName)
                    || string.IsNullOrEmpty(character.Pseudo))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(universe) || !IsValidInput(character.UniverseName)
                    || !IsValidInput(character.Pseudo))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

                bool response = await _characterService.CreateNewCharacter(universe, character);
                if (response) { return Ok(); }
                return BadRequest();
                
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
                // Validation of user inputs
                if (string.IsNullOrEmpty(universe) || string.IsNullOrEmpty(characterId))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(universe) || !IsValidInput(characterId))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
        public async Task<IActionResult> GetAllCharactersByFamilyTreeNameNode(string familyTreeName)
        {
            try
            {
                // Validation of user inputs
                if (string.IsNullOrEmpty(familyTreeName))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(familyTreeName))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
                // Validation of user inputs
                if (string.IsNullOrEmpty(relationForCreationDto.EndPoint)
                    || string.IsNullOrEmpty(relationForCreationDto.StratPoint))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(relationForCreationDto.EndPoint)
                    || !IsValidInput(relationForCreationDto.StratPoint))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
                // Validation of user inputs
                if (string.IsNullOrEmpty(familyTreeName)
                    || string.IsNullOrEmpty(characterName))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(familyTreeName)
                    || !IsValidInput(characterName))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
                // Validation of user inputs
                if (string.IsNullOrEmpty(familyTreeName)
                    || string.IsNullOrEmpty(relationForCreationDto.descriptionRelation)
                    || string.IsNullOrEmpty(relationForCreationDto.EndPoint)
                    || string.IsNullOrEmpty(relationForCreationDto.StratPoint))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(familyTreeName)
                    || !IsValidInput(relationForCreationDto.descriptionRelation)
                    || !IsValidInput(relationForCreationDto.EndPoint)
                    || !IsValidInput(relationForCreationDto.StratPoint))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
                // Validation of user inputs
                if (string.IsNullOrEmpty(familyTreeName)
                    || string.IsNullOrEmpty(characterName1)
                    || string.IsNullOrEmpty(characterName2))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(familyTreeName)
                    || !IsValidInput(characterName1)
                    || !IsValidInput(characterName2))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }
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
                // Validation of user inputs
                if (string.IsNullOrEmpty(familyTreeName)
                    || string.IsNullOrEmpty(relationForUpdatingDto.descriptionRelation)
                    || string.IsNullOrEmpty(relationForUpdatingDto.EndPoint)
                    || string.IsNullOrEmpty(relationForUpdatingDto.StratPoint))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(familyTreeName)
                    || !IsValidInput(relationForUpdatingDto.descriptionRelation)
                    || !IsValidInput(relationForUpdatingDto.EndPoint)
                    || !IsValidInput(relationForUpdatingDto.StratPoint))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
                // Validation of user inputs
                if (string.IsNullOrEmpty(familyTreeName)
                    || string.IsNullOrEmpty(character_name)
                    || string.IsNullOrEmpty(relation_description))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(familyTreeName)
                    || !IsValidInput(character_name)
                    || !IsValidInput(relation_description))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }

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
                // Validation of user inputs
                if (string.IsNullOrEmpty(universeId))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(universeId))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }
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
                // Validation of user inputs
                if (string.IsNullOrEmpty(characterId))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                if (!IsValidInput(characterId))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }
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
                // Validation of user inputs
                if (string.IsNullOrEmpty(characterName))
                {
                    return BadRequest("Universe name and character name are required.");
                }

                // Validation of allowed characters in user inputs
                /*if (!IsValidInput(characterName) || !IsValidInput(characterDetails.FirstName)
                    || !IsValidInput(characterDetails.LastName) || !IsValidInput(characterDetails.Pseudo))
                {
                    return BadRequest("Inputs contain disallowed characters.");
                }*/
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