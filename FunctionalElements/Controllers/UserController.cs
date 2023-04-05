using FunctionalElements.Dtos.User;
using FunctionalElements.Models.User;
using FunctionalElements.Services;
using FunctionalElements.Types;
using Microsoft.AspNetCore.Mvc;

namespace FunctionalElements.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{email}")]
    [ProducesResponseType(typeof(GetUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetUserByEMail(string email)
    {
        return EMail.TryCreate(email)
            .Match(
                GetUserByEMail, 
                static error => error.ToBadRequestTaskActionResult());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(int id)
    {
        return (await _userService.GetUserById(id))
            .Match<IActionResult>(Ok, _ => NotFound());
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> AddUser(AddUserDto dto)
    {
        return Models.User.AddUser.TryCreate(dto)
            .Match(
                AddUser, 
                static pairs => pairs.ToBadRequestTaskActionResult());
    }

    private async Task<IActionResult> AddUser(AddUser addUser)
    {

        return (await _userService.AddUser(addUser))
            .Match<IActionResult>(id => Created("user/", id), error => BadRequest(error.ToReadOnlyDictionary()));
    }

    private async Task<IActionResult> GetUserByEMail(EMail email)
    {
        return (await _userService.GetUserByEmail(email))
            .Match<IActionResult>(Ok, _ => NotFound());
    }
}