using System.Text.Json.Serialization;

namespace AtlasGenerator.Models;

public class Manifest
{
    [JsonPropertyName("frames")]
    public required List<FrameData> Frames { get; set; }

    [JsonPropertyName("meta")]
    public required MetaData Meta { get; set; }
}