using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyPets.Models
{
    public class Owner
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("ownerName")]
        public string OwnerName { get; set; }

        [BsonElement("country")]
        public string Country { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("registeredAt")]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [BsonElement("password")]
        public string Password { get; set; }
    }
}
