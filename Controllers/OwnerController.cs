using Microsoft.AspNetCore.Mvc;
using MyPets.Dtos;
using MyPets.Models;
using MyPets.Repositories;

namespace MyPets.Controllers;

public class OwnerController : BaseApiController
{
    private readonly OwnerRepository _ownerRepository;
    public OwnerController(OwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(OwnerDto model)
    {
        var existing = await _ownerRepository.GetByOwnerName(model.OwnerName);
        if (existing != null)
            return BadRequest(existing);
        var dbUser = await _ownerRepository.Insert(new Owner()
        {
            OwnerName = model.OwnerName,
            Password = model.Password,
            Country = model.Country,
            Gender = model.Gender,
            Id = Guid.NewGuid(),
            RegistrationDate = DateTime.UtcNow
        });
        return Ok(new OwnerResponseDto() {
            Id = dbUser.Id,
            Country = dbUser.Country,
            Gender = dbUser.Gender,
            OwnerName = dbUser.OwnerName,
            RegistrationDate = dbUser.RegistrationDate
        });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = (await _ownerRepository.GetAll())
            .Select(x => new OwnerResponseDto()
            {
                Id = x.Id,
                Country = x.Country,
                Gender = x.Gender,
                OwnerName = x.OwnerName,
                RegistrationDate = x.RegistrationDate
            });
        return Ok(users);
    }

    
    
}