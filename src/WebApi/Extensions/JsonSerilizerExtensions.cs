using System.Text.Json;

namespace WebApi.Extensions;

public static class JsonSerializerExtensions
{
    public static string ToJson(this object obj) => JsonSerializer.Serialize(obj, new JsonSerializerOptions
    {
        WriteIndented = true
    });
}