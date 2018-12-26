using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;

namespace OpenglApp.Imageutils
{
    public class ImageUtil
    {
        public static (int Width, int Height, byte[] Data) GetImage(string path)
        {
            Image<Rgba32> image = Image.Load(path);

            image.Mutate(x => x.Flip(FlipMode.Vertical));

            Rgba32[] tempPixels = image.GetPixelSpan().ToArray();

            List<byte> pixels = new List<byte>();

            foreach (Rgba32 p in tempPixels)
            {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
            }

            return (
                Width: image.Width,
                Height: image.Height,
                Data: pixels.ToArray())
            ;
        }
    }
}
