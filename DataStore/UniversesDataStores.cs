using UniverseCreation.API.DataStore.ModelDto;

namespace UniverseCreation.API.DataStore
{
    public class UniversesDataStores
    {
        public List<UniverseDto> Universes { get; set; }
        public static UniversesDataStores Current { get; } = new UniversesDataStores();

        public UniversesDataStores()
        {

            // init dummy data
            Universes = new List<UniverseDto>() {
                new UniverseDto()
                {
                    Id = 1,
                    Title = "Keleana",
                    LiteraryGenre = "Fantasy",
                    /*Characters = new List<CharacterDto>()
                    {
                        new CharacterDto() 
                        { 
                            Id = 1, 
                            Name = "Keleana", 
                            Description = "Reine de Terrasen",
                            Gender = "Femme",
                            EyeColor = "Turquoise",
                            HairColor = "Blonde"

                        },
                        new CharacterDto() 
                        { 
                            Id = 2, 
                            Name = "Rowan", 
                            Description = "Prince Withertorn",
                            Gender = "Homme",
                            EyeColor = "Vert",
                            HairColor = "Blanc"
                        },
                    }*/
                },
                new UniverseDto()
                {
                    Id = 2,
                    Title = "ACOTAR",
                    LiteraryGenre = "Fantasy",
                    /*Characters = new List<CharacterDto>()
                    {
                        new CharacterDto()
                        {
                            Id = 1,
                            Name = "Feyre",
                            Description = "High Lady de la court des ténébres"
                        },
                        new CharacterDto()
                        {
                            Id = 2,
                            Name = "Rowan",
                            Description = "High Lord de la court des ténébres"
                        }
                    }*/
                }
            };
        }
    }
}
