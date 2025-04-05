namespace AtlasGenerator.Models;

public class Manifest
{
    public required Dictionary<string, FrameData> Frames { get; set; }
    public required MetaData Meta { get; set; }
}