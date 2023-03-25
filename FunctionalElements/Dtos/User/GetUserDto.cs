namespace FunctionalElements.Dtos.User;

public sealed class GetUserDto
{
    public int Id { get; set; }

    public string EMail { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}