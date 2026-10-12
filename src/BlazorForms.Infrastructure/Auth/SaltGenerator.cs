using System.Security.Cryptography;

namespace BlazorForms.Infrastructure.Auth;

/// <summary>
/// Helper methods for generating cryptographically secure random salts.
/// </summary>
public static class SaltGenerator
{
    /// <summary>
    /// Generates a random salt as a byte array.
    /// </summary>
    /// <param name="size">The length of the salt in bytes. Defaults to 32. Must be a positive value.</param> 
    /// <returns>A byte array containing cryptographically secure random bytes.</returns>
    private static byte[] GenerateSalt(int size = 32)
    {
        byte[] salt = new byte[size];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        return salt;
    }

    /// <summary>
    /// Generates a cryptographically secure random salt and returns it encoded as a Base64 string.
    /// </summary>
    /// <param name="size">The length of the salt in bytes. Defaults to 32. Must be a positive value.</param>
    /// <returns>A Base64-encoded representation of the generated salt.</returns>
    public static string GenerateSaltBase64(int size = 32)
    {
        var salt = GenerateSalt(size);
        return Convert.ToBase64String(salt);
    }
}
