using System;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenglApp
{
    public class Shader: IDisposable
    {
        readonly int _handle;

        public Shader(string vertexShaderPath, string fragPath)
        {
            string vertexShaderSource = LoadSource(vertexShaderPath);

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(vertexShader, vertexShaderSource);

            GL.CompileShader(vertexShader);

            string infoLogVert = GL.GetShaderInfoLog(vertexShader);
            if (infoLogVert != string.Empty)
                Console.WriteLine(infoLogVert);

            string fragmentShaderSource = LoadSource(fragPath);
            var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragmentShaderSource);
            GL.CompileShader(FragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(vertexShader);
            if (infoLogFrag != System.String.Empty)
                System.Console.WriteLine(infoLogFrag);

            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, FragmentShader);

            GL.LinkProgram(_handle);

            string infoLogLink = GL.GetShaderInfoLog(vertexShader);
            if (infoLogLink != System.String.Empty)
                System.Console.WriteLine(infoLogLink);

            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(vertexShader);
        }


        public void Use()
        {
            GL.UseProgram(_handle);
        }


        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(_handle, name);

            if (location == -1)
            {
                throw new ArgumentException("uniform name not found");
            }

            GL.Uniform1(location, value);
        }

        
        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(_handle, name);

            if (location == -1)
            {
                throw new ArgumentException("uniform name not found");
            }

            GL.UniformMatrix4(location, true, ref matrix);
        }


        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(_handle, attribName);
        }


        private string LoadSource(string path)
        {
            string readContents;

            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }

            return readContents;
        }


        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                GL.DeleteProgram(_handle);

                _disposedValue = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(_handle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
