using FunctionalElements.Types;

namespace FunctionalElements.Models.User;

public sealed record GetUser(UserId Id, EMail EMail, NonEmpty50CharsString FirstName, NonEmpty50CharsString LastName)
{
    // ReSharper disable once UnusedMember.Local
    private GetUser()
        : this(default, null!, null!, null!)
    {
    }
}