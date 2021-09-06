using OpenglApp.Imageutils;
using System.Collections.Generic;

namespace OpenglApp.SampleObject
{
    public class Map : ObjectBase
    {
        public Map()
        {
            var map = ImageUtil.GetImage("map.png");

            var vertices = new List<float>();
            var indices = new List<uint>();


            //var map = new { Width = 5, Height = 4 };

            for (var z = 0; z < map.Height; z++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var height = map.Data[(z * map.Width + x) * 4] / 50.0f;

                    vertices.AddRange(new[]
                        {x / 10.0f, height, z / 10.0f, (float) x / map.Width, (float) z / map.Height});
                }
            }

            for (uint x = 0; x < (uint) map.Width - 1; x++)
            {
                for (uint z = 0; z < (uint) map.Height - 1; z++)
                {
                    indices.AddRange(new uint[]
                        {z * (uint) map.Width + x, z * (uint) map.Width + x + 1, (z + 1) * (uint) map.Width + x});

                    indices.AddRange(new uint[]
                    {
                        z * (uint) map.Width + x + 1, (z + 1) * (uint) map.Width + x + 1, (z + 1) * (uint) map.Width + x
                    });
                }
            }

            Vertices = vertices.ToArray();
            Indices = indices.ToArray();
        }
    }
}