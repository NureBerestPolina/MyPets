namespace MyPets.Dtos;

public class PetWithOwner
{
    public Guid Id { get; set; }

    public string name { get; set; }

    public string breed { get; set; }

    public string specie { get; set; }

    public string ownerName { get; set; }

    public DateTime addedAt { get; set; }

    public DateTime? updatedAt { get; set; }
}