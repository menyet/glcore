using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenglApp.SampleObject
{
    public class RoadSegment : IObject
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

        public RoadSegment(float size, float sideWalkRatio, float sideWalkHeight)
        {
            var half = size / 2.0f;
            var sidewalkSize = half * sideWalkRatio;



            _vertices = new []
            {
                //Position          Texture coordinates
                -half, sideWalkHeight, -half, 0.0f, 0.0f, // bottom left
                -half, sideWalkHeight, half, 0.0f, 1.0f, // top left 

                -half + sidewalkSize, sideWalkHeight, -half, 0.5f * sideWalkRatio, 0.0f, // top right
                -half + sidewalkSize, sideWalkHeight, half, 0.5f * sideWalkRatio, 1.0f, // bottom right

                -half + sidewalkSize, 0, -half, 0.25f, 0.0f, // top right
                -half + sidewalkSize, 0, half, 0.25f, 1.0f, // bottom right

                half - sidewalkSize, 0, -half, 0.75f, 0.0f, // top right
                half - sidewalkSize, 0, half, 0.75f, 1.0f, // bottom right
               
                half - sidewalkSize, sideWalkHeight, -half, 1.0f - 0.5f * sideWalkRatio, 0.0f, // top right
                half - sidewalkSize, sideWalkHeight, half, 1.0f - 0.5f * sideWalkRatio, 1.0f, // bottom right

                half, sideWalkHeight, -half, 1.0f, 0.0f, // bottom left
                half, sideWalkHeight, half, 1.0f, 1.0f, // top left 
            };

            _indices = new uint[]
            {
                0, 1, 3, 0, 3, 2,
                2, 3, 5, 2, 5, 4,
                4, 5, 7, 4, 7, 6,
                6, 7, 9, 6, 9, 8,
                8, 9, 11, 8, 11, 10
            };

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