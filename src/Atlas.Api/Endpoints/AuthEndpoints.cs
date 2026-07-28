using Atlas.Api.Contracts;
using Atlas.Api.Security;
using Atlas.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", async (LoginRequest request, UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, TokenService tokens, CancellationToken ct) =>
        {
            var user = await users.FindByEmailAsync(request.Email);
            if (user is null || !user.IsActive) return Results.Unauthorized();
            var result = await signIn.CheckPasswordSignInAsync(user, request.Password, true);
            if (!result.Succeeded) return Results.Unauthorized();
            return Results.Ok(await tokens.CreateAsync(user, ct));
        }).AllowAnonymous();

        group.MapPost("/register", async (RegisterRequest request, UserManager<ApplicationUser> users) =>
        {
            var user = new ApplicationUser { UserName = request.Email, Email = request.Email, DisplayName = request.DisplayName, CompanyId = request.CompanyId };
            var result = await users.CreateAsync(user, request.Password);
            if (result.Succeeded)
                return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Email, user.DisplayName });

            var errors = result.Errors
                .GroupBy(x => x.Code)
                .ToDictionary(x => x.Key, x => x.Select(e => e.Description).ToArray());
            return Results.ValidationProblem(errors);
        }).RequireAuthorization(policy => policy.RequireRole("SystemAdministrator", "CompanyAdministrator"));

        group.MapPost("/refresh", async (RefreshRequest request, TokenService tokens, CancellationToken ct) =>
        {
            var response = await tokens.RefreshAsync(request.RefreshToken, ct);
            return response is null ? Results.Unauthorized() : Results.Ok(response);
        }).AllowAnonymous();

        return app;
    }
}
