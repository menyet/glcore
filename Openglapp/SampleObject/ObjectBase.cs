using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenglApp.SampleObject
{
    public class ObjectBase : IObject
    {
        public Vector3 Position { get; set; }

        protected float[] Vertices { get; set; }
        protected uint[] Indices { get; set; }

        protected Shader Shader { get; set; }
        protected Texture Texture { get; set; }
        protected Texture Texture2 { get; set; }

        protected int ElementBufferObject { get; set; }
        protected int VertexBufferObject { get; set; }
        protected int VertexArrayObject { get; set; }

        public void Init()
        {
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices,
                BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices,
                BufferUsageHint.StaticDraw);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);

            //shader.vert has been modified. Take a look at it after the explanation in OnRenderFrame.
            Shader = new Shader("shader.vert", "shader.frag");
            Shader.Use();

            //Texture = new Texture("road_road_0021_01_tiled.png");
            Texture = new Texture("road_road_0021_01_tiled.png");
            Texture.Use(TextureUnit.Texture0);

            Texture2 = new Texture("road_road_0021_01_tiled.png");
            Texture2.Use(TextureUnit.Texture1);

            Shader.SetInt("texture0", 0);
            Shader.SetInt("texture1", 1);

            int vertexLocation = Shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);


            int texCoordLocation = Shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));
        }

        public void Draw(Matrix4 view, Matrix4 projection, Matrix4 model)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BindVertexArray(VertexArrayObject);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);



            Texture.Use(TextureUnit.Texture0);
            Texture2.Use(TextureUnit.Texture1);
            Shader.Use();

            //Then, we pass all of these matrices to the vertex shader.
            //You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects

            //IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            //They are then transposed properly when passed to the shader.
            //If you pass the individual matrices to the shader and multiply there, you have to do in the order "model, view, projection",
            //but if you do it here and then pass it to the vertex, you have to do it in order "projection, view, model".
            Shader.SetMatrix4("model", model);
            Shader.SetMatrix4("view", view);
            Shader.SetMatrix4("projection", projection);

            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Unload()
        {
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            Shader.Dispose();
            Texture.Dispose();
            // Texture2.Dispose();
        }
    }
}