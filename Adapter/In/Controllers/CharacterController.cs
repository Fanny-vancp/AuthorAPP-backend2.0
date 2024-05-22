using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.DataStore.ModelDto;
using UniverseCreation.API.DataStore;
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


        [HttpGet("/api/families_trees/{family_treeName}/characters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CharacterNodeDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCharactersByFamilyTreeNameNode(string family_treeName)
        {
            try
            {
                var characters = await _characterService.FindAllCharactersFromFamilyTree(family_treeName);
                return Ok(characters);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters for the family name : {family_treeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }


        [HttpPost("/api/families_trees/{family_treeName}/characters")]
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

        [HttpDelete("/api/families_trees/{family_treeName}/characters/{characterName}")]
        public async Task<IActionResult> DeleteCharacterFromFamilyTree(string family_treeName, string characterName)
        {
            try
            {
                await _characterService.RemoveCharacterToFamilyTree(family_treeName, characterName);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while deleting characters from the family name : {family_treeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPost("/api/families_trees/{family_treeName}/characters/relation")]
        public async Task<IActionResult> AddRelationBetweenCharacters(string family_treeName, 
            [FromBody] RelationForCreationDto relationForCreationDto)
        {
            try
            {
                await _characterService.InsertRelationBetweenCharacters(relationForCreationDto.StratPoint, 
                    relationForCreationDto.EndPoint, relationForCreationDto.descriptionRelation);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while creating relation between two characters from the family name : {family_treeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpDelete("/api/families_trees/{family_treeName}/characters/{characterName1}/relation/{characterName2}")]
        public async Task<IActionResult> DeleteRelationBetweenCharacter(string characterName1, string characterName2, string family_treeName)
        {
            try
            {
                await _characterService.RemoveRelationBetweenCharacters(characterName1, characterName2);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while removing relation between two characters from the family name : {family_treeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPatch("/api/families_trees/{family_treeName}/characters/relation")]
        public async Task<IActionResult> PatchRelationBetweenCharacters(string family_treeName, [FromBody] RelationForUpdatingDto relationForUpdatingDto)
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
                _logger.LogCritical($"Exception while updating relation between two characters from the family name : {family_treeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpGet("/api/families_trees/{family_treeName}/characters/{character_name}/relation/{relation_description}")]
        public async Task<IActionResult> GetRelationForCharacter(string family_treeName, string character_name, string relation_description)
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
                _logger.LogCritical($"Exception while getting characters with a relation {relation_description} for the character {character_name} for the family name name : {family_treeName}.",
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



        /*[HttpGet("details")]
        public ActionResult<IEnumerable<CharacterDto>> GetCharacters(int universe)
        {
            try
            {
                // find universe
                var universeFind = UniversesDataStores.Current.Universes.FirstOrDefault(c => c.Id == universe);

                if (universeFind == null)
                {
                    _logger.LogInformation($"Universe with id {universe} was'nt found when accessing characters.");
                    return NotFound();
                }

                var charactersList = new List<CharacterDto>();
                charactersList = CharactersDataStores.Current.Characters.Where(c => c.IdUniverse == universe)
                                .ToList();

                return Ok(charactersList);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters for universe with id {universe}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }
        
        [HttpGet("details/{characterId}", Name = "GetCharacters")]
        public ActionResult<CharacterDto> GetCharacter(int universe, int characterId)
        {
            int universeId = universe;
            try
            {
                // find universe
                var universeFind = UniversesDataStores.Current.Universes.FirstOrDefault(c => c.Id == universeId);

                if (universeFind == null)
                {
                    _logger.LogInformation($"Universe with id {universeId} was'nt found when accessing characters.");
                    return NotFound();
                }

                // find character
                var character = CharactersDataStores.Current.Characters.FirstOrDefault(c => c.Id == characterId);

                if (character == null)
                {
                    _logger.LogInformation($"Character with id {characterId} was'nt found when accessing characters.");
                    return NotFound();
                }

                return Ok(character);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters for character with id {characterId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
            
        }

        [HttpPost("details")]
        public ActionResult<CharacterDto> CreationCharacter(int universe, [FromBody] CharacterForCreationDto character)
        {
            int universeId = universe;
            try
            {
                // find universe
                var universeFind = UniversesDataStores.Current.Universes.FirstOrDefault(c => c.Id == universeId);

                if (universeFind == null)
                {
                    _logger.LogInformation($"Universe with id {universeId} was'nt found when creating character.");
                    return NotFound();
                }

                // to be improved
                var maxCharacter = CharactersDataStores.Current.Characters.Max(p => p.Id);

                var finalCharacter = new CharacterDto()
                {
                    Id = ++maxCharacter,
                    IdUniverse = universeId,
                    Pseudo = character.Pseudo
                    //Name = character.Name,
                    //Description = character.Description
                };

                CharactersDataStores.Current.Characters.Add(finalCharacter);

                return CreatedAtRoute("GetCharacters",
                    new
                    {
                        universeId = universeId,
                        characterId = finalCharacter.Id
                    },
                    finalCharacter);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while creation character for universe with id {universeId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPut("details/{characterId}")]
        public ActionResult UpdateCharacter(int universe, int characterId,
            [FromBody] CharacterForUpdateDto character)
        {
            int universeId = universe;
            try
            {
                // find universe
                var universeFind = UniversesDataStores.Current.Universes.FirstOrDefault(c => c.Id == universeId);

                if (universeFind == null)
                {
                    _logger.LogInformation($"Universe with id {universeId} was'nt found when updating character.");
                    return NotFound();
                }

                // find character to update
                var characterFromStore = CharactersDataStores.Current.Characters.FirstOrDefault(c => c.Id == characterId);

                if (characterFromStore == null)
                {
                    _logger.LogInformation($"Character with id {characterId} was'nt found when updating character.");
                    return NotFound();
                }
                characterFromStore.IdUniverse = universeId;
                characterFromStore.Pseudo = character.Pseudo;
                characterFromStore.Name = character.Name;
                characterFromStore.Description = character.Description;
                characterFromStore.Gender = character.Gender;
                characterFromStore.HairColor = character.HairColor;
                characterFromStore.EyeColor = character.EyeColor;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while updating character for character with id {characterId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpPatch("details/{characterId}")]
        public ActionResult PartiallyUdpateCharacter(int universe, int characterId,
            [FromBody] JsonPatchDocument<CharacterForUpdateDto> patchDocument)
        {
            int universeId = universe;
            try
            {
                // find universe
                var universeFind = UniversesDataStores.Current.Universes.FirstOrDefault(c => c.Id == universeId);

                if (universeFind == null)
                {
                    _logger.LogInformation($"Universe with id {universeId} was'nt found when updating character.");
                    return NotFound();
                }

                // find character to update
                var characterFromStore = CharactersDataStores.Current.Characters.FirstOrDefault(c => c.Id == characterId);

                if (characterFromStore == null)
                {
                    _logger.LogInformation($"Character with id {characterId} was'nt found when updating character.");
                    return NotFound();
                }


                var characterToPatch =
                    new CharacterForUpdateDto()
                    {
                        IdUniverse = characterFromStore.IdUniverse,
                        Pseudo = characterFromStore.Pseudo,
                        Name = characterFromStore.Name,
                        Description = characterFromStore.Description,
                        Gender = characterFromStore.Gender,
                        HairColor = characterFromStore.HairColor,
                        EyeColor = characterFromStore.EyeColor,
                    };

                patchDocument.ApplyTo(characterToPatch, ModelState);

                // check the enter properties
                if (!ModelState.IsValid)
                {
                    _logger.LogInformation($"Character with id {characterId} was'nt valid when updating character.");
                    return BadRequest(ModelState);
                }

                // check the enter properties after a update
                if (!TryValidateModel(characterToPatch))
                {
                    _logger.LogInformation($"Character with id {characterId} was'nt valid when updating character.");
                    return BadRequest(ModelState);
                }

                // replace de value
                characterFromStore.IdUniverse = characterToPatch.IdUniverse;
                characterFromStore.Pseudo = characterToPatch.Pseudo;
                characterFromStore.Name = characterToPatch.Name;
                characterFromStore.Description = characterToPatch.Description;
                characterFromStore.Gender = characterToPatch.Gender;
                characterFromStore.HairColor = characterToPatch.HairColor;
                characterFromStore.EyeColor = characterToPatch.EyeColor;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while updating character for character with id {characterId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }

        [HttpDelete("details/{characterId}")]
        public ActionResult DeleteCharacter(int universe, int characterId)
        {
            int universeId = universe;
            try
            {
                // find universe
                var universeFind = UniversesDataStores.Current.Universes.FirstOrDefault(c => c.Id == universeId);

                if (universeFind == null)
                {
                    _logger.LogInformation($"Universe with id {universeId} was'nt found when deleting character.");
                    return NotFound();
                }

                // find character to update
                var characterFromStore = CharactersDataStores.Current.Characters.FirstOrDefault(c => c.Id == characterId);

                if (characterFromStore == null)
                {
                    _logger.LogInformation($"Character with id {characterId} was'nt found when deleting character.");
                    return NotFound();
                }

                // delete permanently 
                CharactersDataStores.Current.Characters.Remove(characterFromStore);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while deleting character for character with id {characterId}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }*/
    }
}
