using Core.Sdk;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class CharacterController : ControllerBase
{

    [HttpPost("create")]
    public void CreateCharacter(CharacterDto dto)
    {    

    }
}