using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenglApp
{
    public class Shader: IDisposable
    {
        int _handle;

        public Shader(string vertPath, string fragPath)
        {
            int VertexShader;
            int FragmentShader;

            string VertexShaderSource = LoadSource(vertPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(VertexShader, VertexShaderSource);

            GL.CompileShader(VertexShader);

            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != System.String.Empty)
                System.Console.WriteLine(infoLogVert);

            string FragmentShaderSource = LoadSource(fragPath);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(VertexShader);
            if (infoLogFrag != System.String.Empty)
                System.Console.WriteLine(infoLogFrag);

            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, VertexShader);
            GL.AttachShader(_handle, FragmentShader);

            GL.LinkProgram(_handle);

            string infoLogLink = GL.GetShaderInfoLog(VertexShader);
            if (infoLogLink != System.String.Empty)
                System.Console.WriteLine(infoLogLink);

            GL.DetachShader(_handle, VertexShader);
            GL.DetachShader(_handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
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
