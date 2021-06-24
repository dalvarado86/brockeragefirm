namespace Application.Common.Interfaces
{
    /// <summary>
    /// IUserAccessor.
    /// </summary>
    public interface IUserAccessor
    {
        /// <summary>
        /// Gets the current user name.
        /// </summary>
        /// <returns>The current user name.</returns>
        string GetCurrentUsername();
    }
}
