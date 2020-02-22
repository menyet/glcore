using OpenglApp.Utils;
using System;

namespace OpenglApp.SampleObject
{
    public struct Vector
    {
        public Vector(float x, float y, float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }

        public float Y { get; set; }
        public float Z { get; set; }

        public static Vector operator+(Vector a, Vector b)
        {
            return new Vector
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
        }

        public static Vector operator *(Vector a, float b)
        {
            return new Vector
            {
                X = a.X * b,
                Y = a.Y * b,
                Z = a.Z * b
            };
        }

        public static Vector operator *(float a, Vector b)
        {
            return b * a;
        }

        public static Vector operator /(Vector a, float b)
        {
            return a * (1/b);
        }

        public float Size => (float)Math.Sqrt(X.Square() + Y.Square() + Z.Square());

        public Vector NormalizedVector => this / Size;

    }
}