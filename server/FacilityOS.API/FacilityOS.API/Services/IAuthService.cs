namespace FacilityOS.API.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Generates a JWT token for the given user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A JWT token as a string.</returns>
        string GenerateToken(string userId);
        /// <summary>
        /// Validates the provided JWT token.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        bool ValidateToken(string token);
        /// <summary>
        /// Gets the user ID from the provided JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>The user ID if the token is valid; otherwise, null.</returns>
        string? GetUserIdFromToken(string token);
    }
}
