using System.ComponentModel.DataAnnotations;

namespace UniverseCreation.API.DataStore.ModelDto
{
    public class UniverseForCreationDto
    {
        [Required]
        public required string Title { get; set; }
        public required string LiteraryGenre { get; set; }
    }
}
