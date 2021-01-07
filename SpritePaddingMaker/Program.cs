using ImageMagick;
using System;
using System.IO;

namespace SpritePaddingMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string filename = args[0];
                PaddedImageBuilder builder = new PaddedImageBuilder
                {
                    Progress = new Progress<int>(p => Console.WriteLine($"Loading.. {p}%")),
                    Settings = ParseArguments(args),
                };

                using MagickImage image = new MagickImage(filename);

                var result = builder.Render(image);
                result.Write(Path.GetFileNameWithoutExtension(filename) + "-padded.png");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static PaddingSettings ParseArguments(string[] arguments)
        {
            var tileSize = arguments[1];
            var padding = arguments[2];

            var tileSizeParts = tileSize.ToLower().Split('x');
            var paddingParts = padding.ToLower().Split('x');

            return new PaddingSettings
            {
                TileX = int.Parse(tileSizeParts[0]),
                TileY = int.Parse(tileSizeParts[1]),
                PaddingX = int.Parse(paddingParts[0]),
                PaddingY = int.Parse(paddingParts[1]),
            };
        }
    }
}
