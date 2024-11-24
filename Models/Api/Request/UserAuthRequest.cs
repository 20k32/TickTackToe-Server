using Server.Models;

namespace PrintMe.Server.Models.Api.ApiRequest;

public sealed class UserAuthRequest : INullCheck
{
    public string UserName { get; init; }
    public string Password { get; init; }
    public UserAuthRequest(string userName, string password) 
        => (UserName, Password) = (userName, password);

    public bool IsNull() => string.IsNullOrWhiteSpace(UserName)
                            || string.IsNullOrWhiteSpace(Password);
}