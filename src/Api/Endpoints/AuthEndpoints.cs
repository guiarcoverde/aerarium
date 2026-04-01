namespace Aerarium.Api.Endpoints;

using Aerarium.Api.Contracts;
using Aerarium.Application.Common;
using Microsoft.AspNetCore.Identity;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest request,
            UserManager<IdentityUser> userManager,
            ITokenService tokenService) =>
        {
            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        { "Identity", errors.ToArray() }
                    });
            }

            var token = tokenService.GenerateToken(user.Id, user.Email!);
            return Results.Created("/api/auth", new AuthResponse(token, user.Email!));
        });

        group.MapPost("/login", async (
            LoginRequest request,
            UserManager<IdentityUser> userManager,
            ITokenService tokenService) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
                return Results.Problem(
                    title: "Invalid credentials",
                    statusCode: StatusCodes.Status401Unauthorized);

            var token = tokenService.GenerateToken(user.Id, user.Email!);
            return Results.Ok(new AuthResponse(token, user.Email!));
        });
    }
}
