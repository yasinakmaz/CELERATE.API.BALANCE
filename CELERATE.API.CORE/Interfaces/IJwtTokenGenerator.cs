using CELERATE.API.CORE.Interfaces;
using CELERATE.API.CORE.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CELERATE.API.CORE.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, List<Permission> permissions);
    }
}
