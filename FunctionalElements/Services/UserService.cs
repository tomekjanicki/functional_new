using System.Data.SqlClient;
using Dapper;
using FunctionalElements.Models.User;
using FunctionalElements.Types;
using OneOf;
using OneOf.Types;

namespace FunctionalElements.Services;

public sealed class UserService
{
    private readonly string _connectionString;

    public UserService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("connectionString")!;
    }

    public async Task<OneOf<GetUser, NotFound>> GetUserByEmail(EMail email)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QuerySingleOrDefaultAsync<GetUser>("SELECT [Id], [EMail], [FirstName], [LastName] FROM [dbo].[User] WHERE [EMail] = @email",
            new { email = email.Value });

        return result != null ? result : new NotFound();
    }

    public async Task<OneOf<GetUser, NotFound>> GetUserById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QuerySingleOrDefaultAsync<GetUser>("SELECT [Id], [EMail], [FirstName], [LastName] FROM [dbo].[User] WHERE [Id] = @id",
            new { id });

        return result != null ? result : new NotFound();
    }

    public async Task<OneOf<int, Error<string>>> AddUser(AddUser addUser)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QuerySingleAsync<int>("INSERT [dbo].[User] ([EMail], [FirstName], [LastName]) VALUES(@email, @firstName, @lastName); SELECT SCOPE_IDENTITY()",
               new { email = addUser.EMail.Value, firstName = addUser.FirstName.Value, lastName = addUser.LastName.Value });

            return result;
        }
        catch (SqlException e)
        {
            if (e.Number != 2601)
            {
                throw;
            }

            return new Error<string>("Email should be unique");
        }
    }
}