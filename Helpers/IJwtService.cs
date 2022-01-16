using System.IdentityModel.Tokens.Jwt;

namespace JWTAuthProject.Helpers
{
    public interface IJwtService
    {
        string Generate(int id);
        JwtSecurityToken Verify(string jwt);
    }
}
