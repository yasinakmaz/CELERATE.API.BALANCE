using System.Security;

namespace CELERATE.API.Infrastructure.Firebase
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, List<Permission> permissions);
    }
}
