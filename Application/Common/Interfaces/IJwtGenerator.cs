using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(ApplicationUser user);
    }
}
