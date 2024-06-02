namespace UniverseCreation.API.Application.Domain.Model
{
    public class CharacterNodeDto
    {
        public required string name { get; set; }
        public List<string> children { get; set; }
        public List<string> parents { get; set; }
        public List<string> married { get; set; }
        public List<string> divorced { get; set; }
        public List<string> couple { get; set; }
        public List<string> amant { get; set; }
        public required int level { get; set; }
    }
}
