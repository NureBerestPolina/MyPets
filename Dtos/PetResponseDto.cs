using MongoDB.Bson.Serialization.Attributes;

namespace MyPets.Dtos
{
    public class PetResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public string Specie { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
