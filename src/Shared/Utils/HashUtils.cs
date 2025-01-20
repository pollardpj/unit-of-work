using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Shared.Json;

namespace Shared.Utils;

public static class HashUtils
{
    public static string GetHash<T>(T input)
    {
        // Check input validity
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Input cannot be null or empty.");
        }
        
        var inputAsJson = JsonSerializer.Serialize(input, JsonHelpers.DefaultOptions);
        
        return GetHash(inputAsJson);
    }
    
    public static string GetHash(string input)
    {
        // Check input validity
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input), "Input cannot be null or empty.");
        }
        
        // Convert string to bytes
        var inputBytes = Encoding.UTF8.GetBytes(input);

        // Compute hash
        var hashBytes = SHA256.HashData(inputBytes);

        // Convert hash to hexadecimal string
        var hashStringBuilder = new StringBuilder();
        foreach (var b in hashBytes)
        {
            hashStringBuilder.Append(b.ToString("x2"));
        }

        return hashStringBuilder.ToString();
    }
}