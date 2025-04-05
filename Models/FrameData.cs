using System.Text.Json.Serialization;

namespace AtlasGenerator.Models;

public class FrameData
{
    [JsonPropertyName("filename")]
    public required string FileName { get; set; }

    [JsonPropertyName("frame")]
    public required Rect Frame { get; set; }

    [JsonPropertyName("sourceSize")]
    public required Size SourceSize { get; set; }

    [JsonPropertyName("spriteSourceSize")]
    public Rect SpriteSourceSize => new Rect
    {
        X = 0,
        Y = 0,
        W = SourceSize.W,
        H = SourceSize.H
    };

    [JsonPropertyName("pivot")]
    public Point Pivot { get; set; } = new Point
    {
        X = 0.5f,
        Y = 0.5f
    };

    [JsonPropertyName("trimmed")]
    public bool Trimmed { get; set; } = false;

    [JsonPropertyName("rotated")]
    public bool Rotated { get; set; } = false;
}