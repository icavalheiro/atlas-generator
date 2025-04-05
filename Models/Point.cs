using System.Text.Json.Serialization;

namespace AtlasGenerator.Models;

public class Point
{
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }
}