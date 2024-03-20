using MongoDB.Bson.Serialization.Attributes;

namespace MyPets.Dtos
{
    public class PetDto
    {
        public string Name { get; set; }
        public string Breed { get; set; }
        public string Specie { get; set; }
    }
}
