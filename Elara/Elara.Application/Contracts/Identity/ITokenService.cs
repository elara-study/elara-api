namespace Elara.Application.Contracts.Identity
{
    public interface ITokenService
    {
        string CreateToken(Guid userId, string email, string fullName, string role);
    }
}
