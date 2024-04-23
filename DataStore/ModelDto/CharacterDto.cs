namespace UniverseCreation.API.DataStore.ModelDto
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public int IdUniverse { get; set; }
        public string Pseudo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Gender { get; set; }
        public string EyeColor { get; set; }
        public string HairColor { get; set; }
    }
}
