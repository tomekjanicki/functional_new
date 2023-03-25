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
        var emailResult = EMail.TryCreate(email);

        return emailResult.Match(GetUserByEMail, error => Task.FromResult<IActionResult>(BadRequest(error.ToReadOnlyDictionary())));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _userService.GetUserById(id);

        return result.Match<IActionResult>(Ok, _ => NotFound());
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> AddUser(AddUserDto dto)
    {
        var addUserResult = Models.User.AddUser.TryCreate(dto);

        return addUserResult.Match(AddUser, pairs => Task.FromResult<IActionResult>(BadRequest(pairs)));
    }

    private async Task<IActionResult> AddUser(AddUser addUser)
    {
        var result = await _userService.AddUser(addUser);

        return result.Match<IActionResult>(id => Created("user/", id), error => BadRequest(error.ToReadOnlyDictionary()));
    }

    private async Task<IActionResult> GetUserByEMail(EMail email)
    {
        var result = await _userService.GetUserByEmail(email);

        return result.Match<IActionResult>(Ok, _ => NotFound());
    }
}