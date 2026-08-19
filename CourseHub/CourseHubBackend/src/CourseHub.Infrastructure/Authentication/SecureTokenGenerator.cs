using System.Security.Cryptography;
using CourseHub.Application.Common.Interfaces;

namespace CourseHub.Infrastructure.Authentication;

/// <summary>
/// Generates cryptographically secure random tokens using
/// RandomNumberGenerator (not System.Random), Base64Url-encoded so the
/// result is safe to place directly in a URL query string.
/// </summary>
public class SecureTokenGenerator : ISecureTokenGenerator
{
    private const int TokenSizeInBytes = 64;

    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
