using Microsoft.AspNetCore.Mvc;
using MyPets.Dtos;
using MyPets.Models;
using MyPets.Repositories;
using MyPets.Services;

namespace MyPets.Controllers;

public class OwnerController : BaseApiController
{
    private readonly OwnerRepository _ownerRepository;
    private readonly PetRepository _petRepository;
    private readonly CurrentUserService _currentUserService;
    public OwnerController(OwnerRepository ownerRepository, CurrentUserService currentUserService, PetRepository petRepository)
    {
        _ownerRepository = ownerRepository;
        _currentUserService = currentUserService;
        _petRepository = petRepository;
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


    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var result = await _ownerRepository.Login(model);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        if (changePasswordDto.NewPassword != changePasswordDto.NewPasswordConfirm ||
            changePasswordDto.NewPassword == changePasswordDto.OldPassword) return Ok(false);
        var a = await _ownerRepository.ChangePassword(new Guid(_currentUserService.Id()), changePasswordDto.NewPassword);
        return Ok(a);

    }

    [HttpGet]
    public async Task<IActionResult> Search(string substring)
    {
        var a = await _petRepository.Search(new Guid(_currentUserService.Id()), substring);
        return Ok(a);
    }
    
    [HttpGet]
    public async Task<IActionResult> JIJA()
    {
        var a = _currentUserService.Email();
        return Ok(a);
    }
    
}