using System;
using System.Collections.Generic;
using System.Text;

namespace OpenglApp
{
    public struct Point
    {
        public Point(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }
    }
}
