using OpenglApp.Imageutils;
using OpenglApp.OSM;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace OpenglApp.SampleObject
{
    public static class Ext {
        public static void Ad<T>(this IList<T> list,  params T[] items)
        {
            foreach (var item in items)
            {
                list.Add(item);

            }
        } 
    }

    public class OsMap : ObjectBase
    {
        public OsMap()
        {
            OsmLoader.Load(@"Resources/map.osm", out var nodes, out var ways);


            var map = ImageUtil.GetImage("Resources/map.png");

            foreach (var n in nodes)
            {
                n.Value.lat = n.Value.lat - 48.0928479;
                n.Value.lon = n.Value.lon - 17.1170996;
            }

            // lat = "48.0928479" lon = "17.1170996"

            var vertices = new List<float>();
            var indices = new List<uint>();

            var squareSize = 0.1f;
            var scale = 2000.0f;

            uint ind = 0;
            foreach (var way in ways)
            {
                for (int i = 0; i < way.nodes.Length - 1; i++)
                {
                    var node1 = nodes[way.nodes[i]];
                    var node2 = nodes[way.nodes[i + 1]];

                    var dY = node2.lat - node1.lat;
                    var dX = node2.lon - node1.lon;

                    var size = Math.Sqrt(dY * dY + dX * dX);

                    var v1 = new Vector((float)node1.lon, 0, (float)node1.lat) * scale;
                    var v2 = new Vector((float)node2.lon, 0, (float)node2.lat) * scale;

                    var dV = (v1 - v2).NormalizedVector * squareSize;

                    var ortho = new Vector(dV.Z, 0, -dV.X);

                    var n1 = v1 + ortho;
                    var n2 = v1 - ortho;
                    var n3 = v2 + ortho;
                    var n4 = v2 - ortho;



                    vertices.Ad(n1.X, n1.Y, n1.Z, 0.0f, 0.0f);
                    vertices.Ad(n2.X, n2.Y, n2.Z, 0.0f, 1.0f);
                    vertices.Ad(n3.X, n3.Y, n3.Z, 1.0f, 0.0f);
                    vertices.Ad(n4.X, n4.Y, n4.Z, 1.0f, 1.0f);

                    indices.Ad(ind, ind+1, ind+2, ind+2, ind + 1, ind +3);

                    ind += 4;
                }
            }




            //foreach (var n in nodes)
            //{
            //    vertices.Ad((float)n.Value.lat * scale + squareSize, 0, (float)n.Value.lon * scale + squareSize, 0, 0);
            //    vertices.Ad((float)n.Value.lat * scale + squareSize, 0, (float)n.Value.lon * scale - squareSize, 0, 0);
            //    vertices.Ad((float)n.Value.lat * scale - squareSize, 0, (float)n.Value.lon * scale + squareSize, 0, 0);
            //    vertices.Ad((float)n.Value.lat * scale - squareSize, 0, (float)n.Value.lon * scale - squareSize, 0, 0);
            //}

            //for (uint i = 0; i < nodes.Count; i++)
            //{
            //    var start = i * 4;
            //    indices.Ad(start, start + 1, start + 2, start + 2, start + 1, start + 3);
            //}

            Vertices = vertices.ToArray();
            Indices = indices.ToArray();

        }

        

        
        public void Unload()
        {
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            Shader?.Dispose();
            Texture?.Dispose();
            Texture2?.Dispose();
        }
    }
}

