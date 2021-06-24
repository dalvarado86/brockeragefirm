using Domain.Entities;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// IJwtGenerator.
    /// </summary>
    public interface IJwtGenerator
    {
        /// <summary>
        /// Creates a token for a specific user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The generated token.</returns>
        string CreateToken(ApplicationUser user);
    }
}
