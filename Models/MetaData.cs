using System.Text.Json.Serialization;

namespace AtlasGenerator.Models;

public class MetaData
{
    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("size")]
    public required Size Size { get; set; }

    [JsonPropertyName("app")]
    public static string App = "AtlasGenerator .net";
}