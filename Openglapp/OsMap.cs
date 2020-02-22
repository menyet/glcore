
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using OpenglApp.OSM;
using OpenglApp.Utils;
using OpenTK.Graphics.OpenGL;

namespace OpenglApp.SampleObject
{
    using OpenglApp;
    using OpenglApp.Imageutils;
    using OpenTK;
    using OpenTK.Graphics.OpenGL4;

    public static class Ext {
        public static void Ad<T>(this IList<T> list,  params T[] items)
        {
            foreach (var item in items)
            {
                list.Add(item);

            }
        } 
    }

    public class OsMap : IObject
    {
        public Vector3 Position { get; set; }

        Shader _shader;
        Texture _texture;
        Texture _texture2;

        int _elementBufferObject;
        int _vertexBufferObject;
        int _vertexArrayObject;

        private float[] _vertices;
        private uint[] _indices;

        // private IEnumerable<float> GetLayer()


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

            _vertices = vertices.ToArray();
            _indices = indices.ToArray();

        }

        public void Init()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
                BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            //shader.vert has been modified. Take a look at it after the explanation in OnRenderFrame.
            _shader = new Shader("shader.vert", "shader.frag");
            _shader.Use();

            //_texture = new Texture("road_road_0021_01_tiled.png");
            _texture = new Texture("road_road_0021_01_tiled.png");
            _texture.Use(TextureUnit.Texture0);

            _texture2 = new Texture("road_road_0021_01_tiled.png");
            _texture2.Use(TextureUnit.Texture1);

            _shader.SetInt("texture0", 0);
            _shader.SetInt("texture1", 1);

            int vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);


            int texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));
        }

        public void Draw(Matrix4 view, Matrix4 projection, Matrix4 model)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BindVertexArray(_vertexArrayObject);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            _texture.Use(TextureUnit.Texture0);
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            //Then, we pass all of these matrices to the vertex shader.
            //You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects

            //IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            //They are then transposed properly when passed to the shader.
            //If you pass the individual matrices to the shader and multiply there, you have to do in the order "model, view, projection",
            //but if you do it here and then pass it to the vertex, you have to do it in order "projection, view, model".
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Unload()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            _shader?.Dispose();
            _texture?.Dispose();
            _texture2?.Dispose();
        }
    }
}

