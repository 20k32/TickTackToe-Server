using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Server.Models.Auth;
using Shared.Api.Models;

namespace Server.Core.Auth;

internal sealed class TokenGenerator
{
    private readonly Options _options;
    public TokenGenerator(Options options) => _options = options;

    public string GetForSuccessLoginResult(SuccessLoginResult loginResult)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.SecureBase64Span.ToArray());

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = GenerateClaims(loginResult),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = credentials
        };

        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }


    private static ClaimsIdentity GenerateClaims(SuccessLoginResult loginResult)
    {
        var claims = new ClaimsIdentity();

        claims.AddClaim(new Claim(CustomClaimTypes.USER_ID, loginResult.Id.ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Name, loginResult.UserName));

        return claims;
    }

}