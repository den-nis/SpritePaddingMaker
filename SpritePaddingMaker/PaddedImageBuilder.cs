using ImageMagick;
using System;
using System.Diagnostics;
using System.Linq;

namespace SpritePaddingMaker
{
    public class PaddedImageBuilder
    {
        public PaddingSettings Settings { get; set; } = new PaddingSettings();
        public IProgress<int> Progress { get; set; } = new Progress<int>();

        public MagickImage Render(MagickImage image)
        {
            if (image.Width % Settings.TileX != 0 || image.Height % Settings.TileY != 0)
                throw new InvalidOperationException($"Cannot divide resolution {image.Width}x{image.Height} in to tiles of {Settings.TileX}x{Settings.TileY}");

            var result = BuildImageWithPadding(image);
            return result;
        }

        private MagickImage BuildImageWithPadding(MagickImage source)
        {
            int tilesWide = source.Width / Settings.TileX;
            int tilesHigh = source.Height / Settings.TileY;

            int newImageWidth = source.Width + Settings.PaddingX * tilesWide * 2;
            int newImageHeight = source.Height + Settings.PaddingY * tilesHigh * 2;

            MagickImage result = new MagickImage(MagickColors.Transparent, newImageWidth, newImageHeight);

            IMagickImage<ushort>[]? tiles = source.CropToTiles(Settings.TileX, Settings.TileY).ToArray();

            for (int y = 0; y < tilesHigh; y++)
            {
                Progress.Report((int)(100 * ((float)(y+1) / tilesHigh)));
                for (int x = 0; x < tilesWide; x++)
                {
                    using var subImage = tiles[x + y * tilesWide];
                    subImage.BorderColor = MagickColors.Transparent;
                    subImage.Border(Settings.PaddingX, Settings.PaddingY);
                    using var subImagePixels = subImage.GetPixels();

                    ExtendEdge(subImagePixels, subImage.Width, subImage.Height);
                    subImage.Write($"z{x}x{y}.png");

                    result.Composite(subImage, x * subImage.Width, y * subImage.Height, CompositeOperator.SrcOver);
                }
            }

            return result;
        }

        private void ExtendEdge(IPixelCollection<ushort> pixels, int width, int height)
        {
            ushort[]? brushXLeft  = pixels.GetArea(Settings.PaddingX, 0, 1, height);
            ushort[]? brushXRight = pixels.GetArea(width - 1 - Settings.PaddingX, 0, 1, height);
            for (int i = 0; i < Settings.PaddingX; i++)
            {
                pixels.SetArea(i          , 0, 1, height, brushXLeft);
                pixels.SetArea(width-1 - i, 0, 1, height, brushXRight);
            }

            ushort[]? brushYLeft  = pixels.GetArea(0, Settings.PaddingY, width, 1);
            ushort[]? brushYRight = pixels.GetArea(0, height - 1 - Settings.PaddingY, width, 1);
            for (int i = 0; i < Settings.PaddingY; i++)
            {
                pixels.SetArea(0, i          , width, 1, brushYLeft);
                pixels.SetArea(0, height-1 - i, width, 1, brushYRight);
            }
        }
    }
}
