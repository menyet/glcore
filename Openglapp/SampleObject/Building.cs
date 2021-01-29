using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenglApp.SampleObject
{
    public class Building : IObject
    {
        public Vector3 Position { get; set; }
        public long Id { get; set; }
        public long[] Nodes { get; set; }
        public string Type { get; set; }
        public float? Levels { get; set; }

        float[] _vertices;

        uint[] _indices;


        Shader _shader;
        Texture _texture;
        Texture _texture2;

        int _elementBufferObject;
        int _vertexBufferObject;
        int _vertexArrayObject;

        public Building()
        {
            float radius = 1;
            List<float> data = new List<float>();
            List<uint> indices = new List<uint>();

            const uint layersNum = 100;
            const uint segmentCount = 200;

            for (uint height = 0; height <= layersNum; height++)
            {
                if (height == 0)
                {
                    data.AddRange(new[] { 0.0f, radius, 0.0f, 0.0f, 0.0f });
                }
                else if (height == layersNum)
                {
                    data.AddRange(new[] { 0.0f, -radius, 0.0f, 1.0f, 1.0f });
                }
                else
                {
                    var verticalAngle = MathHelper.DegreesToRadians(height * 180.0f / layersNum);
                    var y = radius * Math.Cos(verticalAngle);

                    var coef = Math.Sin(verticalAngle);



                    for (uint segment = 0; segment < segmentCount; segment++)
                    {
                        var radAngle = MathHelper.DegreesToRadians(segment * 360.0f / segmentCount);

                        var z = radius * Math.Sin(radAngle) * coef;
                        var x = radius * Math.Cos(radAngle) * coef;

                        data.AddRange(new[]
                        {
                            (float)x,
                            (float)y,
                            (float)z,
                            0.5f,
                            (float) (height % 2)
                        });
                    }
                }
            }

            for (uint height = 0; height < layersNum; height++)
            {
                for (uint segment = 0; segment < segmentCount; segment++)
                {
                    if (height == 0)
                    {
                        indices.AddRange(new uint[] { 0, segment + 1, (segment + 1) % segmentCount + 1 });
                    }
                    else if (height == layersNum - 1)
                    {
                        indices.AddRange(new uint[]
                        {
                            (uint) data.Count / 5 - 1,
                            1 + (height - 1) * segmentCount + segment,
                            1 + (height - 1) * segmentCount + (segment + 1) % segmentCount
                        });
                    }
                    else
                    {
                        indices.AddRange(new uint[]
                        {
                            1 + (height - 1) * segmentCount + segment,
                            1 + (height - 1) * segmentCount + (segment + 1) % segmentCount,
                            1 + height * segmentCount + segment,

                            1 + (height - 1) * segmentCount + (segment + 1) % segmentCount,
                            1 + (height) * segmentCount + segment,
                            1 + (height) * segmentCount + (segment + 1) % segmentCount
                        });
                    }
                }
            }

            _vertices = data.ToArray();

            _indices = indices.ToArray();
        }

        public void Init()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            //shader.vert has been modified. Take a look at it after the explanation in OnRenderFrame.
            _shader = new Shader("shader.vert", "shader.frag");
            _shader.Use();

            _texture = new Texture("container.png");
            _texture.Use(TextureUnit.Texture0);

            _texture2 = new Texture("awesomeface.png");
            _texture2.Use(TextureUnit.Texture1);

            _shader.SetInt("texture0", 0);
            _shader.SetInt("texture1", 1);

            int vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);


            int texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
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
