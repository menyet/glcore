using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenglApp.SampleObject;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace OpenglApp
{
    //This is where all OpenGL code will be written.
    //OpenTK allows for several functions to be overriden to extend functionality; this is how we'll be writing code.
    class Game : GameWindow
    {       


        //We create a double to hold how long has passed since the program was opened.
        double _time;

        //Then, we create two matrices to hold our view and projection. They're initialized at the bottom of OnLoad.
        //The view matrix is what you might consider the "camera". It represents the current viewport in the window.
        private readonly Camera _camera = new Camera(0, 0, 3);
        private IObject _object;

        //This represents how the vertices will be projected. It's hard to explain through comments,
        //so check out the web version for a good demonstration of what this does.
        Matrix4 _projection;
        
        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }
        
        private IEnumerable<Vector> Casteljau(List<Vector> points)
        {
            for (float t = 0; t <= 1; t += 0.01f)
            {
                yield return GetPoint(points.Count - 1, 0, t);
            }

            Vector GetPoint(int level, int i, float t)
            {
                if (level == 0)
                {
                    return points[i];
                }

                var p1 = GetPoint(level - 1, i, t);
                var p2 = GetPoint(level - 1, i + 1, t);

                return (1.0f - t) * p1 + t * p2;
            }
        }
        
        protected override void OnLoad(EventArgs e)
        {
            Keyboard.KeyDown += (sender, ev) => _keyState[ev.Key] = true;
            Keyboard.KeyUp += (sender, ev) => _keyState[ev.Key] = false;

            Mouse.Move += (sender, ev) =>
            {
                // var center = PointToScreen(new Point(Width / 2, Height / 2));
                
                _camera.RotationY += (ev.X - Width / 2) * 0.001f;

                _camera.RotationX += (ev.Y - Height / 2) * 0.001f;

                if (_camera.RotationX < -3.0f / 2.0f) _camera.RotationX = -3.0f / 2.0f;

                if (_camera.RotationX > 3.0f / 2.0f) _camera.RotationX = 3.0f / 2.0f;

                // Console.WriteLine($"{ev.X} {center.X} {Width / 2}");
            };

            

            //We enable depth testing here. If you try to draw something more complex than one plane without this,
            //you'll notice that polygons further in the background will occasionally be drawn over the top of the ones in the foreground.
            //Obviously, we don't want this, so we enable depth testing. We also clear the depth buffer in GL.Clear over in OnRenderFrame.
            GL.Enable(EnableCap.DepthTest);

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


            var points = new List<Vector>
            {
                new Vector(0, 0, 0),
                new Vector(0, 0, 3.0f),
                new Vector(3.0f, 2.0f, 3.0f),
                new Vector(4.0f, 2.0f, 3.0f),
                new Vector(6.0f, 0.0f, 3.0f),
                new Vector(6.0f, 0.0f, 0.0f)                
            };

            _object = new Street(Casteljau(points)
            .Select(_ => new StreetEndConfig
            {
                Position = _
            }).ToList(), 1, 0.4f, 0.05f);

           
            _object.Position = new Vector3(0.0f, 0.0f, -3.0f);
            _object.Init();

            //For the matrix, we use a few parameters.
            //  Field of view. This determines how much the viewport can see at once. 45 is considered the most "realistic" setting, but most video games nowadays use 90
            //  Aspect ratio. This should be set to Width / Height.
            //  Near-clipping. Any vertices closer to the camera than this value will be clipped.
            //  Far-clipping. Any vertices farther away from the camera than this value will be clipped.
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), Width / Height, 0.1f, 50.0f);

            // _projection = Matrix4.CreateOrthographic(10, 6, 0.1f, 50.0f);

            //Now, head over to OnRenderFrame to see how we setup the model matrix

            base.OnLoad(e);
        }

        private readonly IDictionary<Key, bool> _keyState = new Dictionary<Key, bool>();


        int _frame = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //_viewZ = (float)Math.Sin(_time / 100);


            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
            _time += 10* 4.0 * e.Time;

            //We clear the depth buffer in addition to the color buffer
            // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            //Finally, we have the model matrix. This determines the position of the model.
            Matrix4 model = Matrix4.Identity
                  * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(/*_time / 2*/0.0)) * Matrix4.CreateTranslation(_object.Position);


            _object.Draw(_camera.Matrix, _projection, model);
            
            Context.SwapBuffers();

            if (_frame++ % 10 == 0)
            {
                Task.Run(() =>
                {
                    Console.Clear();
                    Console.WriteLine($"View rotation X:      {_camera.RotationX}");
                    Console.WriteLine($"View rotation Y:      {_camera.RotationY}");
                    Console.WriteLine($"View:               {_camera.X} {_camera.Y} {_camera.Z}");
                    Console.WriteLine($"Projection:         {(float)MathHelper.DegreesToRadians(_time)}");
                });
            }


            base.OnRenderFrame(e);
        }

        private bool IsKeyPressed(Key key)
        {
            return _keyState.TryGetValue(key, out var s) && s;
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Nothing to do!
            OpenTK.Point center = new OpenTK.Point(Width / 2, Height / 2);
            OpenTK.Point mousePos = PointToScreen(center);
            OpenTK.Input.Mouse.SetPosition(mousePos.X, mousePos.Y);

            if (IsKeyPressed(Key.Escape))
            {
                Exit();
            }

            const float speed = 2.3f;

            if (IsKeyPressed(Key.Q))
            {
                _camera.RotationY -= speed * 0.02f;
            }

            if (IsKeyPressed(Key.E))
            {
                _camera.RotationY += speed * 0.02f;
            }


            if (IsKeyPressed(Key.A))
            {
                _camera.Move(-speed * (float) e.Time, 0, 0);
            }

            if (IsKeyPressed(Key.D))
            {
                _camera.Move(speed * (float)e.Time, 0, 0);
            }

            if (IsKeyPressed(Key.W))
            {
                _camera.Move(0, 0, speed * (float) e.Time);
            }

            if (IsKeyPressed(Key.S))
            {
                _camera.Move(0, 0, -speed * (float)e.Time);
            }

            if (IsKeyPressed(Key.Up))
            {
                var p = _object.Position;
                p.Z -= speed * (float)e.Time;
                _object.Position = p;
            }

            if (IsKeyPressed(Key.Down))
            {
                var p = _object.Position;
                p.Z += speed * (float)e.Time;
                _object.Position = p;
            }

            if (IsKeyPressed(Key.Left))
            {
                var p = _object.Position;
                p.X -= speed * (float)e.Time;
                _object.Position = p;
            }

            if (IsKeyPressed(Key.Right))
            {
                var p = _object.Position;
                p.X += speed * (float)e.Time;
                _object.Position = p;
            }

            base.OnUpdateFrame(e);
        }


        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }


        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            _object.Unload();

            base.OnUnload(e);
        }
    }
}
