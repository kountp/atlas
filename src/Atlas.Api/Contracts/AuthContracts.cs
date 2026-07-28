namespace Atlas.Api.Contracts;

public sealed record LoginRequest(string Email, string Password);
public sealed record RegisterRequest(string Email, string Password, string DisplayName, Guid? CompanyId);
public sealed record RefreshRequest(string RefreshToken);
public sealed record TokenResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAtUtc, string RefreshToken, DateTimeOffset RefreshTokenExpiresAtUtc);
