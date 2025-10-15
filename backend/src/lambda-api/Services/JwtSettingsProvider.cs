using System.Text.Json;
using lambda_api.Controllers; // For JwtSettings

public static class JwtSettingsProvider
{
    private static JwtSettings? _jwtSettings;

    public static JwtSettings GetJwtSettings()
    {
        if (_jwtSettings != null)
            return _jwtSettings;

        var jwtJson = Environment.GetEnvironmentVariable("jwt");
        if (string.IsNullOrEmpty(jwtJson))
            throw new InvalidOperationException("JWT environment variable is not set.");

        JwtSettings? jwtSettings;
        try
        {
            jwtSettings = JsonSerializer.Deserialize<JwtSettings>(jwtJson);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to parse JWT environment variable.", ex);
        }

        if (jwtSettings == null ||
            string.IsNullOrEmpty(jwtSettings.SecretKey) ||
            string.IsNullOrEmpty(jwtSettings.Issuer) ||
            string.IsNullOrEmpty(jwtSettings.Audience))
        {
            throw new InvalidOperationException("JWT environment variable is missing required fields.");
        }

        _jwtSettings = jwtSettings;
        return _jwtSettings;
    }
}