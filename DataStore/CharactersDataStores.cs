using UniverseCreation.API.DataStore.ModelDto;

namespace UniverseCreation.API.DataStore
{
    public class CharactersDataStores
    {
        public List<CharacterDto> Characters { get; set; }
        public static CharactersDataStores Current { get; } = new CharactersDataStores();

        public CharactersDataStores()
        {

            // init dummy data
            Characters = new List<CharacterDto>() {
                new CharacterDto()
                {
                    Id = 1,
                    IdUniverse = 1,
                    Pseudo = "Keleana",
                    Name = "Aelin Galathinus",
                    Description = "Reine de Terrasen",
                    Gender = "Femme",
                    EyeColor = "Turquoise",
                    HairColor = "Blonde"

                },
                new CharacterDto()
                {
                    Id = 2,
                    IdUniverse = 1,
                    Pseudo = "Rowan",
                    Name = "Rowan Whithertorn",
                    Description = "Prince Withertorn",
                    Gender = "Homme",
                    EyeColor = "Vert",
                    HairColor = "Blanc"
                },
                new CharacterDto()
                {
                    Id = 3,
                    IdUniverse = 2,
                    Pseudo = "Feyre",
                    Name = "Feyre Archeron",
                    Description = "Grand Dame de la court de la nuit",
                    Gender = "Femme",
                    EyeColor = "Marron",
                    HairColor = "Brune"

                },
                new CharacterDto()
                {
                    Id = 4,
                    IdUniverse = 2,
                    Pseudo = "Rhysand",
                    Name = "Seigneur Rhysand",
                    Description = "Grand Seigneur de la court de la nuit",
                    Gender = "Homme",
                    EyeColor = "Violet",
                    HairColor = "Noir"
                },
                new CharacterDto()
                {
                    Id = 5,
                    IdUniverse = 2,
                    Pseudo = "Azriel",
                    Name = "Azriel",
                    Description = "ShadowSinger de la court des ténébres",
                    Gender = "Homme",
                    EyeColor = "Marron",
                    HairColor = "Brun"
                },
            };
        }
    }
}
