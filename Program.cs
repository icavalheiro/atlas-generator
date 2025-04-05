using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.Json;
using AtlasGenerator.Models;
using Size = AtlasGenerator.Models.Size;
using Point = SixLabors.ImageSharp.Point;

if (args.Length != 1)
{
    Console.WriteLine("Usage: AtlasGenerator <folder with images>");
    return;
}

var folder = args[0];
var filesInFolder = Directory.GetFiles(folder, "*", SearchOption.AllDirectories).
    Where(x => Path.GetFileNameWithoutExtension(x) != "atlas");

var images = new List<(Image Image, string FileName)>();

// Load all images
foreach (var file in filesInFolder)
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
var frames = new List<FrameData>();
int currentY = 0;

foreach (var row in rows)
{
    int currentX = 0;
    int rowHeight = row.Max(img => img.Image.Height);

    foreach (var (image, fileName) in row)
    {
        atlasImage.Mutate(ctx => ctx.DrawImage(image, new Point(currentX, currentY), 1));
        var frameName = Path.GetFileNameWithoutExtension(fileName);

        frames.Add(new FrameData
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
            },
            FileName = frameName,
        });

        currentX += image.Width;
    }

    currentY += rowHeight;
}

// Save outputs
var atlasPath = Path.Combine(folder, "atlas.png");
atlasImage.Save(atlasPath);

var manifestPath = Path.Combine(folder, "atlas.json");
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
File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, jsonSerializerOptions));

Console.WriteLine("Atlas generated successfully!");
Console.WriteLine($"PNG:\n{atlasPath}\n");
Console.WriteLine($"JSON:\n{manifestPath}");









