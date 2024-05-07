using MongoDB.Bson.Serialization.Attributes;

namespace MyPets.Dtos
{
    public class OwnerDto
    {
        public string OwnerName { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
    }
}
