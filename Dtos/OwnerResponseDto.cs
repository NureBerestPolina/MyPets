using MongoDB.Bson.Serialization.Attributes;

namespace MyPets.Dtos
{
    public class OwnerResponseDto
    {
        public Guid Id { get; set; }
        public string OwnerName { get; set; }

        public string Country { get; set; }

        public string Gender { get; set; }

        public DateTime RegistrationDate { get; set; } 
    }
}
