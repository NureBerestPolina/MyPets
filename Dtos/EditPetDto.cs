namespace MyPets.Dtos;

public class EditPetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Breed { get; set; }
    public string Specie { get; set; }
}