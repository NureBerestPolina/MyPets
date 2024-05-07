using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace MyPets.Models
{
    public class Pet
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("breed")]
        public string Breed { get; set; }

        [BsonElement("specie")]
        public string Specie { get; set; }

        [BsonElement("ownerId")]
        public Guid OwnerId { get; set; }

        [BsonElement("addedAt")]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        
    }
}
