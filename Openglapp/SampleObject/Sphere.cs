using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace OpenglApp.SampleObject
{
    public class Sphere : ObjectBase
    {
        public Sphere(float radius)
        {
            List<float> data = new List<float>();
            List<uint> indices = new List<uint>();

            const uint layersNum = 30;
            const uint segmentCount = 30;

            for (uint height = 0; height <= layersNum; height++)
            {
                var verticalAngle = MathHelper.DegreesToRadians(height * 180.0f / layersNum);
                var y = radius * Math.Cos(verticalAngle);

                var coef = Math.Sin(verticalAngle);

                for (uint segment = 0; segment <= segmentCount; segment++)
                {
                    var radAngle = MathHelper.DegreesToRadians(segment * 360.0f / segmentCount);

                    var z = radius * Math.Sin(radAngle) * coef;
                    var x = radius * Math.Cos(radAngle) * coef;

                    var texX = (float) segment / segmentCount;
                    var texY = 1.0f - (float) height / layersNum;

                    if (height == layersNum)
                    {
                        x = 0;
                        z = 0;
                    }

                    data.AddRange(new[]
                    {
                        (float) x,
                        (float) y,
                        (float) z,
                        texX,
                        texY
                    });
                }
            }

            for (uint height = 0; height <= layersNum; height++)
            {
                for (uint segment = 0; segment < segmentCount; segment++)
                {
                    indices.AddRange(new uint[]
                    {
                        (height - 1) * (segmentCount + 1) + segment,
                        (height - 1) * (segmentCount + 1) + (segment + 1) % (segmentCount + 1),
                        height * (segmentCount + 1) + segment,

                        (height - 1) * (segmentCount + 1) + (segment + 1) % (segmentCount + 1),
                        (height) * (segmentCount + 1) + (segment + 1) % (segmentCount + 1),
                        (height) * (segmentCount + 1) + segment,
                    });
                }
            }

            Vertices = data.ToArray();

            Indices = indices.ToArray();
        }
    }

    public interface IObject
    {
        void Draw(Matrix4 view, Matrix4 projection, Matrix4 model);

        void Init();


        Vector3 Position { get; set; }
        void Unload();
    }
}
