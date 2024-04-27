using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API.DataStore.ModelDto;
using UniverseCreation.API.DataStore;
using UniverseCreation.API.Application.Domain.Model;

namespace UniverseCreation.API.Adapter.In.Controllers
{
    [ApiController]
    [Route("api/universes/{universe}/characters")]
    public class CharactersController : ControllerBase
    {
        private readonly ILogger<CharactersController> _logger;
        private readonly ICharacterRepositoryGraph _characterRepositoryGraph;
        private readonly IFamilyTreeRepositoryGraph _familyTreeRepositoryGraph;

        public CharactersController(ILogger<CharactersController> logger, ICharacterRepositoryGraph characterRepository, IFamilyTreeRepositoryGraph familyTreeRepositoryGraph)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _characterRepositoryGraph = characterRepository;
            _familyTreeRepositoryGraph = familyTreeRepositoryGraph;
        }

        
        [HttpGet("{characterName}")]
        public async Task<IActionResult> SearchCharacterByName(string universe, string characterName)
        {
            try
            {
                var characters = await _characterRepositoryGraph.SearchCharacterBythename(universe, characterName);

                /*if (characters == null)
                {
                    characters = await _characterRepositoryGraph.
                }*/

                return Ok(characters);
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
                var characters = await _characterRepositoryGraph.MatchAllCharactersByUniverseName(universe);
                return Ok(characters);
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
        public async Task<IActionResult> GetAllCharactersFromFamilyTreeName(string family_treeName)
        {
            try
            {
                // check if the family tree exist
                var familyTreeFinded = await _familyTreeRepositoryGraph.FindFamilyTree(family_treeName);
                if (familyTreeFinded == null)
                {
                    _logger.LogInformation($"Family Tree with the name {family_treeName} was'nt found when accessing characters.");
                    return NotFound();
                }


                var characters = await _characterRepositoryGraph.MatchAllCharactersFromFamilyTree(family_treeName);
                return Ok(characters);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters for the family name name : {family_treeName}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }


        [HttpPost("/api/families_trees/{family_treeName}/characters")]
        public async Task<IActionResult> AddCharacterInFamilyTreeName([FromBody] RelationForCreationDto relationForCreationDto)
        {
            try
            {
                // check if the family tree exist
                var familyTreeFinded = await _familyTreeRepositoryGraph.FindFamilyTree(relationForCreationDto.EndPoint);
                if (familyTreeFinded == null)
                {
                    _logger.LogInformation($"Family Tree with the name {relationForCreationDto.EndPoint} was'nt found when adding character in family tree.");
                    return NotFound();
                }

                // check if the character exist
                var characterFinded = await _characterRepositoryGraph.FindCharacter(relationForCreationDto.StratPoint);
                if (characterFinded == null)
                {
                    _logger.LogInformation($"Character with the name {relationForCreationDto.StratPoint} was'nt found when adding character in familyTree.");
                    return NotFound();
                }


                await _characterRepositoryGraph.AddCharacterToFamilyTree(relationForCreationDto.EndPoint, relationForCreationDto.StratPoint);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while adding characters for the family name name : {relationForCreationDto.StratPoint}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }


        /*[HttpGet("relative")]
        public async Task<IActionResult> GetAllCharactersAndRelative(string universe)
        {
            try
            {
                var charactersAndRelative = await _characterRepository.MatchAllCharactersWithHisRelative(universe);
                return Ok(charactersAndRelative);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting characters for universe with the name : {universe}.",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }

        }*/


        [HttpGet("details")]
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

        }
    }
}
