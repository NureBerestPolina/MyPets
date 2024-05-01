using Microsoft.AspNetCore.Mvc;
using MyPets.Dtos;
using MyPets.Models;
using MyPets.Repositories;
using MyPets.Services;

namespace MyPets.Controllers;

public class PetController : BaseApiController
{
    private readonly PetRepository _petRepository;
    private readonly CurrentUserService _currentUserService;
    
    public PetController(PetRepository petRepository, CurrentUserService currentUserService)
    {
        _petRepository = petRepository;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PetDto petDto)
    {
        var result = await _petRepository.Insert(new Pet
        {
            AddedAt = DateTime.Now,
            Breed = petDto.Breed,
            Id = Guid.NewGuid(),
            Name = petDto.Name,
            OwnerId = new Guid(_currentUserService.Id()),
            Specie = petDto.Specie,
            UpdatedAt = DateTime.Now
        });
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetByUserId()
    {
        var userId = new Guid(_currentUserService.Id());
        var result = await _petRepository.GetByOwnerId(userId);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _petRepository.GetById(id);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _petRepository.Delete(id);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] EditPetDto editPetDto)
    {
        var result = await _petRepository.Edit(editPetDto);
        return Ok(result);
    }
}