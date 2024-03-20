using Microsoft.AspNetCore.Mvc;

namespace MyPets.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public abstract class BaseApiController : ControllerBase{ }