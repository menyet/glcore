using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Openglapp;
using OpenglApp.SampleObject;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenglApp
{
    //This is where all OpenGL code will be written.
    //OpenTK allows for several functions to be overriden to extend functionality; this is how we'll be writing code.
    class Game : GameWindow
    {
        private int Width { get; set; }
        private int Height { get; set; }

        //We create a double to hold how long has passed since the program was opened.
        double _time;

        //Then, we create two matrices to hold our view and projection. They're initialized at the bottom of OnLoad.
        //The view matrix is what you might consider the "camera". It represents the current viewport in the window.
        private readonly Camera _camera = new Camera(0, 0, 3);
        private List<IObject> _objectList = new List<IObject>();

        //This represents how the vertices will be projected. It's hard to explain through comments,
        //so check out the web version for a good demonstration of what this does.
        Matrix4 _projection;
        Stopwatch _watch = new Stopwatch();

        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = new Vector2i(width, height),
            Title = title
        })
        {
            Width = width;
            Height = height;
            _watch.Start();
        }

        private IObject _skybox;

        protected override void OnLoad()
        {
            Console.Clear();
            Console.WriteLine($"View rotation X:");
            Console.WriteLine($"View rotation Y:");
            Console.WriteLine($"View:");
            Console.WriteLine($"FPS:");


            KeyDown += (ev) => _keyState[ev.Key] = true;
            KeyUp += (ev) => _keyState[ev.Key] = false;
            
            //We enable depth testing here. If you try to draw something more complex than one plane without this,
            //you'll notice that polygons further in the background will occasionally be drawn over the top of the ones in the foreground.
            //Obviously, we don't want this, so we enable depth testing. We also clear the depth buffer in GL.Clear over in OnRenderFrame.
            GL.Enable(EnableCap.DepthTest);

            var m = new OsMap();
            m.Init();
            _objectList.Add(m);

            _skybox = new Sphere(2);
            _skybox.Init();
            //_objectList.Add(_skybox);


            //var c = new SampleSquare();
            //c.Init();
            //_objectList.Add(c);


            //For the view, we don't do too much here. Next tutorial will be all about a Camera class that will make it much easier to manipulate the view.
            //For now, we move it backwards three units on the Z axis.

            // Matrix4.CreatePerspectiveOffCenter(-1, 1, -1, 1, 0.1f, 5.0f, out _view);

            //_object = new SampleSquare();

            //_object = new Street(new List<StreetEndConfig>() {
            //    new StreetEndConfig(),
            //    new StreetEndConfig()
            //{
            //    Position = new Vector(0.0f,0.0f,3.0f)
            //},
            //new StreetEndConfig()
            //{
            //    Position = new Vector(3.0f,0.0f,3.0f)
            //},
            //new StreetEndConfig()
            //{
            //    Position = new Vector(5.0f,0.5f,3.0f)
            //}
            //}, 1, 0.4f, 0.05f);


            //foreach (var obj in new ObjectLoader().LoadObjects())
            //{
            //    _objectList.Add(obj);
            //}

            //For the matrix, we use a few parameters.
            //  Field of view. This determines how much the viewport can see at once. 45 is considered the most "realistic" setting, but most video games nowadays use 90
            //  Aspect ratio. This should be set to Width / Height.
            //  Near-clipping. Any vertices closer to the camera than this value will be clipped.
            //  Far-clipping. Any vertices farther away from the camera than this value will be clipped.
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), Width / Height, 0.1f, 50.0f);

            // _projection = Matrix4.CreateOrthographic(10, 6, 0.1f, 50.0f);

            //Now, head over to OnRenderFrame to see how we setup the model matrix

            base.OnLoad();
        }

        

        private readonly IDictionary<Keys, bool> _keyState = new Dictionary<Keys, bool>();


        long _frame = 0;
        private long _lastTime = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //_viewZ = (float)Math.Sin(_time / 100);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
            _time += 10* 4.0 * e.Time;

            //We clear the depth buffer in addition to the color buffer
            // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _skybox.Position = (_camera.X, _camera.Y, _camera.Z);


            //Finally, we have the model matrix. This determines the position of the model.
            foreach (var obj in _objectList)
            {
                Matrix4 model = Matrix4.Identity
                      * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(/*_time / 2*/0.0)) * Matrix4.CreateTranslation(obj.Position);

                obj.Draw(_camera.Matrix, _projection, model);
            }

            SwapBuffers();

            if (_frame++ % 25 == 0)
            {
                var time = _watch.ElapsedMilliseconds;
                var fps = 25000 / (double) (time - _lastTime);
                _lastTime = time;

                Console.SetCursorPosition(20, 0);
                Console.Write($"{_camera.RotationX}");
                Console.SetCursorPosition(20, 1);
                Console.Write($"{_camera.RotationY}");
                Console.SetCursorPosition(20, 2);
                Console.Write($"{_camera.X} {_camera.Y} {_camera.Z}");
                Console.SetCursorPosition(20, 3);
                Console.Write($"{fps}");
            }

            base.OnRenderFrame(e);
        }

        private bool IsKeyPressed(Keys key)
        {
            return _keyState.TryGetValue(key, out var s) && s;
        }

        private bool _firstPass = true;

        private void HandleMouse()
        {
            var center = new Vector2(Width / 2, Height / 2);

            if (!_firstPass)
            {
                var dX = MouseState.X - center.X;
                var dY = MouseState.Y - center.Y;

                _camera.RotationY += (dX) * 0.01f;
                _camera.RotationX += (dY) * 0.01f;

                if (_camera.RotationX < -3.0f / 2.0f) _camera.RotationX = -3.0f / 2.0f;
                if (_camera.RotationX > 3.0f / 2.0f) _camera.RotationX = 3.0f / 2.0f;
            }
            else
            {
                _firstPass = false;
            }

            Console.SetCursorPosition(20, 4);
            Console.Write($"{MouseState.X} {MouseState.PreviousX} {MouseState.Delta.X}               ");

            MousePosition = center;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Nothing to do!

            HandleMouse();

            if (IsKeyPressed(Keys.Escape))
            {
                Close();
            }

            const float speed = 2.3f;

            if (IsKeyPressed(Keys.Q))
            {
                _camera.RotationY -= speed * 0.02f;
            }

            if (IsKeyPressed(Keys.E))
            {
                _camera.RotationY += speed * 0.02f;
            }

            if (IsKeyPressed(Keys.I))
            {
                _camera.RotationX -= speed * 0.02f;
            }

            if (IsKeyPressed(Keys.K))
            {
                _camera.RotationX += speed * 0.02f;
            }

            if (IsKeyPressed(Keys.A))
            {
                _camera.Move(-speed * (float) e.Time, 0, 0);
            }

            if (IsKeyPressed(Keys.D))
            {
                _camera.Move(speed * (float)e.Time, 0, 0);
            }

            if (IsKeyPressed(Keys.W))
            {
                _camera.Move(0, 0, speed * (float) e.Time);
            }

            if (IsKeyPressed(Keys.S))
            {
                _camera.Move(0, 0, -speed * (float)e.Time);
            }

            // if (IsKeyPressed(Key.Up))
            // {
            //     var p = _object.Position;
            //     p.Z -= speed * (float)e.Time;
            //     _object.Position = p;
            // }

            // if (IsKeyPressed(Key.Down))
            // {
            //     var p = _object.Position;
            //     p.Z += speed * (float)e.Time;
            //     _object.Position = p;
            // }

            // if (IsKeyPressed(Key.Left))
            // {
            //     var p = _object.Position;
            //     p.X -= speed * (float)e.Time;
            //     _object.Position = p;
            // }

            // if (IsKeyPressed(Key.Right))
            // {
            //     var p = _object.Position;
            //     p.X += speed * (float)e.Time;
            //     _object.Position = p;
            // }

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (var obj in _objectList)
            {
                obj.Unload();
            }

            base.OnUnload();
        }
    }
}
