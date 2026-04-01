namespace Aerarium.Application.Common;

public interface ITokenService
{
    string GenerateToken(string userId, string email);
}
