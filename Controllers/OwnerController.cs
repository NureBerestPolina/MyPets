using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GeoJsonObjectModel;
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
            RegistrationDate = DateTime.UtcNow,
            Location = GeoJson.Point(GeoJson.Position(model.Latitude,model.Longtitude))
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
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        try
        {
           var id = new Guid(_currentUserService.Id()) ;
                   var user = (await _ownerRepository.GetById(id));
                   return Ok(user); 
        }
        catch (NullReferenceException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        try
        {
            var result = await _ownerRepository.Login(model);
                    return Ok(result);
        }catch (NullReferenceException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }

    [HttpPut]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
           if (changePasswordDto.NewPassword != changePasswordDto.NewPasswordConfirm ||
                       changePasswordDto.NewPassword == changePasswordDto.OldPassword) return Ok(false);
                   var a = await _ownerRepository.ChangePassword(new Guid(_currentUserService.Id()), changePasswordDto.NewPassword);
                   return Ok(a); 
        }catch (NullReferenceException ex)
        {
            return BadRequest(ex.Message);
        }
        

    }

    [HttpGet]
    public async Task<IActionResult> Search(string substring)
    {
        var a = await _petRepository.Search(substring);
        return Ok(a);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string ownerName, string password)
    {
        var result = await _ownerRepository.Delete(ownerName, password);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GeoGet(string specie, double radius)
    {
        var (latitude, longtitude) = await _ownerRepository.GetLoc(new Guid(_currentUserService.Id()));
        var a = await _ownerRepository.GetNearestOwnersOfSpecie(latitude, longtitude, specie, radius/111);
        return Ok(a);
    }
}