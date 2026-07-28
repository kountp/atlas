using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Atlas.Api.Contracts;
using Atlas.Infrastructure.Identity;
using Atlas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Atlas.Api.Security;

public sealed class TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, AtlasDbContext db)
{
    public async Task<TokenResponse> CreateAsync(ApplicationUser user, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        var accessMinutes = configuration.GetValue("Jwt:AccessTokenMinutes", 30);
        var refreshDays = configuration.GetValue("Jwt:RefreshTokenDays", 14);
        var accessExpires = now.AddMinutes(accessMinutes);
        var refreshExpires = now.AddDays(refreshDays);
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new("company_id", user.CompanyId?.ToString() ?? string.Empty)
        };
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, now.UtcDateTime, accessExpires.UtcDateTime, credentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var plainRefresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        db.RefreshTokens.Add(new RefreshToken { TokenHash = Hash(plainRefresh), UserId = user.Id, ExpiresAtUtc = refreshExpires });
        await db.SaveChangesAsync(ct);
        return new TokenResponse(accessToken, accessExpires, plainRefresh, refreshExpires);
    }

    public async Task<TokenResponse?> RefreshAsync(string plainToken, CancellationToken ct = default)
    {
        var hash = Hash(plainToken);
        var current = await db.RefreshTokens.Include(x => x.User).SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (current is null || !current.IsActive || !current.User.IsActive) return null;
        current.RevokedAtUtc = DateTimeOffset.UtcNow;
        return await CreateAsync(current.User, ct);
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
