namespace Server.Models.Api.Result
{
    public sealed class TokenResult : ResultBase
    {
        public string Token { get; init; }

        public TokenResult(string token, string message, int statusCode) : base(message, statusCode)
            => Token = token;
    }

}
