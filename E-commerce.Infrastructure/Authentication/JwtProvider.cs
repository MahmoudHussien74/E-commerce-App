using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace E_commerce.Infrastructure.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public (string token, int expireMinute) GenerateToken(User user)
    {
        Claim[] claims = [
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Name, user.Name),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


        var tokenDescriptor = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_options.ExpireInMinutes),
                signingCredentials: signingCredentials
                );

        return (token: new JwtSecurityTokenHandler().WriteToken(tokenDescriptor), expireMinute:_options.ExpireInMinutes);
    }
}
