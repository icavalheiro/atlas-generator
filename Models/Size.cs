using System.Text.Json.Serialization;

namespace AtlasGenerator.Models;

public class Size
{
    [JsonPropertyName("w")]
    public int W { get; set; }

    [JsonPropertyName("h")]
    public int H { get; set; }
}