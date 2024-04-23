using System.ComponentModel.DataAnnotations;

namespace UniverseCreation.API.DataStore.ModelDto
{
    public class CharacterForCreationDto
    {
        [Required]
        public int IdUniverse { get; set; }
        public required string Pseudo { get; set; }
    }
}
