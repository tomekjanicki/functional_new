using FunctionalElements.Types;

namespace FunctionalElements.Models.User;

public sealed record GetUser(int Id, EMail EMail, NonEmpty50CharsString FirstName, NonEmpty50CharsString LastName);