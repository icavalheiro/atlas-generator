using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.Json;
using AtlasGenerator.Models;
using Size = AtlasGenerator.Models.Size;

if (args.Length == 0)
{
    Console.WriteLine("Usage: AtlasGenerator <image1> <image2> ...");
    return;
}

try
{
    var images = new List<(Image Image, string FileName)>();

    // Load all images
    foreach (var file in args)
    {
        var image = Image.Load(file);
        images.Add((image, Path.GetFileName(file)));
    }

    // Arrange images into rows of up to 8
    var rows = new List<List<(Image Image, string FileName)>>();
    var currentRow = new List<(Image Image, string FileName)>();

    foreach (var image in images)
    {
        currentRow.Add(image);
        if (currentRow.Count == 8)
        {
            rows.Add(currentRow);
            currentRow = new List<(Image Image, string FileName)>();
        }
    }

    if (currentRow.Count > 0)
        rows.Add(currentRow);

    // Calculate atlas dimensions
    int atlasWidth = 0;
    int atlasHeight = 0;

    foreach (var row in rows)
    {
        int rowWidth = row.Sum(img => img.Image.Width);
        int rowHeight = row.Max(img => img.Image.Height);
        atlasWidth = Math.Max(atlasWidth, rowWidth);
        atlasHeight += rowHeight;
    }

    // Create atlas image
    using var atlasImage = new Image<Rgba32>(atlasWidth, atlasHeight);
    var frames = new Dictionary<string, FrameData>();
    int currentY = 0;

    foreach (var row in rows)
    {
        int currentX = 0;
        int rowHeight = row.Max(img => img.Image.Height);

        foreach (var (image, fileName) in row)
        {
            atlasImage.Mutate(ctx => ctx.DrawImage(image, new Point(currentX, currentY), 1));

            frames[fileName] = new FrameData
            {
                Frame = new Rect
                {
                    X = currentX,
                    Y = currentY,
                    W = image.Width,
                    H = image.Height
                },
                SourceSize = new Size
                {
                    W = image.Width,
                    H = image.Height
                }
            };

            currentX += image.Width;
        }

        currentY += rowHeight;
    }

    // Save outputs
    atlasImage.Save("atlas.png");

    var manifest = new Manifest
    {
        Frames = frames,
        Meta = new MetaData
        {
            Image = "atlas.png",
            Size = new Size
            {
                W = atlasImage.Width,
                H = atlasImage.Height
            }
        }
    };

    var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText("atlas.json", JsonSerializer.Serialize(manifest, jsonSerializerOptions));

    Console.WriteLine("Atlas generated successfully!");
    Console.WriteLine($"PNG: atlas.png");
    Console.WriteLine($"JSON: atlas.json");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}









