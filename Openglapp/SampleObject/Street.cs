using System;
using System.Collections.Generic;
using System.Linq;
using OpenglApp.Utils;

namespace OpenglApp.SampleObject
{
    using OpenglApp;
    using OpenTK;
    using OpenTK.Graphics.OpenGL4;

    public class Street : IObject
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


        public Street(StreetEndConfig startConfig, StreetEndConfig endConfig, float size, float sideWalkRatio, float sideWalkHeight)
        {
            var half = size / 2.0f;
            var sidewalkSize = half * sideWalkRatio;

            var distance = Math.Sqrt(startConfig.Position.X.Square() + startConfig.Position.Y.Square() +
                                     startConfig.Position.Z.Square());

            Vector direction = GetDirection(startConfig, endConfig);


            IEnumerable<float> GetLayer(Vector center, float textureV)
            {
                // var sidedirection = new Vector(direction.Z, direction.Y, -direction.X);
                var sidedirection = new Vector(1.0f, 0.0f, 0.0f);
                var upDirection = new Vector(0.0f, 1.0f, 0.0f);

                var p1 = center - sidedirection * half + upDirection * sideWalkHeight;
                var p2 = center - sidedirection * (half - sidewalkSize) + upDirection * sideWalkHeight;
                var p3 = center - sidedirection * (half - sidewalkSize);

                var p4 = center + sidedirection * (half - sidewalkSize);
                var p5 = center + sidedirection * (half - sidewalkSize) + upDirection * sideWalkHeight;
                var p6 = center + sidedirection * half + upDirection * sideWalkHeight;

                return new[]
                {
                    //center.X - sidedirection.X * half, center.Y + sideWalkHeight, center.Z, 0.0f, textureV, // bottom left
                    p1.X, p1.Y, p1.Z, 0.0f, textureV, // bottom left
                    p2.X, p2.Y, p2.Z, 0.5f * sideWalkRatio, textureV, // top right
                    p3.X, p3.Y, p3.Z, 0.25f, textureV, // top right
                    p4.X, p4.Y, p4.Z, 0.75f, textureV, // top right
                    p5.X, p5.Y, p5.Z, 1.0f - 0.5f * sideWalkRatio, textureV, // top right
                    p6.X, p6.Y, p6.Z, 1.0f, textureV, // bottom left
                };
            }

            var firstLayer = GetLayer(startConfig.Position, 0.0f);
            var secondLayer = GetLayer(endConfig.Position, 1.0f);

            _vertices = firstLayer
                .Concat(secondLayer).ToArray();

            _indices = new uint[]
            {
                0, 6, 7, 0, 7, 1,
                1, 7, 8, 1, 8, 2,
                2, 8, 9, 2, 9, 3,
                3, 9, 10, 3, 10, 4,
                4, 10, 11, 4, 11, 5
            };

        }

        private Vector GetDirection(StreetEndConfig startConfig, StreetEndConfig endConfig)
        {
            var startPoint = startConfig.Position;
            var endPoint = endConfig.Position;

            var v = endPoint - startPoint;

            return v / v.Size;

            //var dX = endPoint.X - startPoint.X;
            //var dY = endPoint.Y - startPoint.Y;
            //var dZ = endPoint.Z - startPoint.Z;

            //var coef = (float)(1.0f / Math.Sqrt(dX.Square() + dY.Square() + dZ.Square()));

            //return new Vector(dX * coef, dY * coef, dZ * coef);
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            //shader.vert has been modified. Take a look at it after the explanation in OnRenderFrame.
            _shader = new Shader("shader.vert", "shader.frag");
            _shader.Use();

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
            GL.BindVertexArray(_vertexArrayObject);

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

            _shader.Dispose();
            _texture.Dispose();
            _texture2.Dispose();
        }
    }
}