using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

public static class ParameterStoreHelper
{
    private static IDictionary<string, object>? _secrets;

    public static async Task LoadSecretsToEnvironmentAsync()
    {
        if (_secrets != null)
            return; // Already loaded

        var region = Environment.GetEnvironmentVariable("REGION");
        var config = new AmazonSimpleSystemsManagementConfig { RegionEndpoint = RegionEndpoint.GetBySystemName(region) };
        using var client = new AmazonSimpleSystemsManagementClient(config);

        // Retrieve a single parameter by name
        string parameterName = $"lambda-secrets-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}"; // Adjust as needed for your environment

        var request = new GetParameterRequest
        {
            Name = parameterName,
            WithDecryption = true
        };

        var response = await client.GetParameterAsync(request);

        // If the parameter value is a JSON object with multiple secrets
        var secrets = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(response.Parameter.Value);

        _secrets = secrets ?? new Dictionary<string, object>();

        foreach (var kvp in _secrets)
        {
            // Convert the value to a string before setting it as an environment variable
            var value = kvp.Value?.ToString();
            Environment.SetEnvironmentVariable(kvp.Key, value);
        }
    }
}
