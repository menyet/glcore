using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace OpenglApp
{
    public class Camera
    {
        public Camera(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float RotationY { get; set; }

        public void Move(float dX, float dY, float dZ)
        {
            X = X + dX * (float)Math.Cos(RotationY) + dZ * (float)Math.Sin(RotationY) * (float)Math.Cos(RotationX);
            Y = Y + dY + (float)Math.Sin(RotationX) * dZ;
            Z = Z - dX * (float)Math.Sin(RotationY) + dZ * (float)Math.Cos(RotationY) * (float)Math.Cos(RotationX);
        }

        public Matrix4 Matrix => Matrix4.CreateTranslation(-X, Y, Z) * Matrix4.CreateFromAxisAngle(Vector3.UnitY, RotationY) * Matrix4.CreateFromAxisAngle(Vector3.UnitX, RotationX);
        public float RotationX { get; set; }
    }
}
