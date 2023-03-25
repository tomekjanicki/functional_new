using FunctionalElements.Dtos.User;
using FunctionalElements.Types;
using FunctionalElements.Types.Collections;
using OneOf;

namespace FunctionalElements.Models.User;

public sealed record AddUser
{
    private AddUser(EMail eMail, NonEmpty50CharsString firstName, NonEmpty50CharsString lastName)
    {
        EMail = eMail;
        FirstName = firstName;
        LastName = lastName;
    }

    public EMail EMail { get; }
    public NonEmpty50CharsString FirstName { get; }
    public NonEmpty50CharsString LastName { get; }

    public static OneOf<AddUser, ReadOnlyDictionary<string, string>> TryCreate(AddUserDto? dto)
    {
        if (dto is null)
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                { string.Empty, "dto is null" }
            });
        }
        var emailResult = EMail.TryCreate(dto.EMail);
        var firstNameResult = NonEmpty50CharsString.TryCreate(dto.FirstName);
        var lastNameResult = NonEmpty50CharsString.TryCreate(dto.LastName);
        if (emailResult.IsT0 && firstNameResult.IsT0 && lastNameResult.IsT0)
        {
            return new AddUser(emailResult.AsT0, firstNameResult.AsT0, lastNameResult.AsT0);
        }
        var errors = new Dictionary<string, string>();
        if (emailResult.IsT1)
        {
            errors.Add(nameof(AddUserDto.EMail), emailResult.AsT1.Value);
        }
        if (firstNameResult.IsT1)
        {
            errors.Add(nameof(AddUserDto.FirstName), firstNameResult.AsT1.Value);
        }
        if (lastNameResult.IsT1)
        {
            errors.Add(nameof(AddUserDto.LastName), lastNameResult.AsT1.Value);
        }

        return new ReadOnlyDictionary<string, string>(errors);
    }
}