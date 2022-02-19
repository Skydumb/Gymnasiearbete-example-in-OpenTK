using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace opentk_example
{
    class Program
    {
        static void Main()
        {
            using Window window = new Window(800, 600, "Window");
            window.Run();
        }
    }
    class Window : GameWindow
    {
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
            // Front Face
             0.5f,  0.5f, 0.5f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.5f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.5f, 0.0f, 1.0f,  // top left
            // Top Face
            -0.5f,  0.5f, -0.5f, 0.0f, 0.0f,  // far top left
            0.5f,  0.5f, -0.5f, 1.0f, 0.0f,  // far top right
            // Bottom Face
            -0.5f,  -0.5f, -0.5f, 0.0f, 1.0f,  // far bottom left
            0.5f,  -0.5f, -0.5f, 1.0f, 1.0f,  // far bottom right
        };
        private readonly uint[] _indices =
        {
            // front
            0, 1, 3,
            1, 2, 3,
            // top
            3, 4, 5,
            0, 3, 5,
            // bottom
            1, 2, 7,
            2, 6, 7,
            // back
            5, 4, 6,
            5, 7, 6,
            // left
            3, 2, 6,
            6, 4, 3,
            // right
            0, 1, 7,
            7, 5, 0,
        };

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;
        private int _elementBufferObject;
        private Texture _texture;
        private Texture _texture2;
        private double _renderTime;
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        private Model _cubePrim;
        private IlluminatedModel _litCubePrim;

        private readonly Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        public Window(int height, int width, string name) :
            base(new GameWindowSettings(), new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
                Title = name
            })
        {

        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            var deltaTime = args.Time;
            if (!IsFocused) return;
            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape)) this.Close();
            // Moving cube
            _cubePrim.transform *= Matrix4.CreateTranslation(0.0f, 0.5f * (float)deltaTime, 0.0f);

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;
            // Camera controls
            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)deltaTime; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)deltaTime; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)deltaTime; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)deltaTime; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)deltaTime; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)deltaTime; // Down
            }

            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.5f, 0.7f, 1f, 1f);

            GL.Enable(EnableCap.DepthTest);

            _cubePrim = Model.CubePrimitive();
            _litCubePrim = Model.SharpCubePrimitve();

            _litCubePrim.lightPositon = _lightPos;
            // Creating and binding VBO
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            // Creation of VAO
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            // Position pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // Texture pointers
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            // Loading textures
            _texture = Texture.LoadFromFile("Resources/hamster.png");
            _texture.Use(TextureUnit.Texture0);
            _texture2 = Texture.LoadFromFile("Resources/container.png");
            _texture2.Use(TextureUnit.Texture1);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // Adding Shaders
            _shader = new Shader("Shaders/TextureShader.vert", "Shaders/TextureShader.frag");
            _shader.Use();
            // Binding textures
            _shader.SetInt("texture0", 0);
            _shader.SetInt("texture1", 1);
            // Creating a camera object and capturing the cursor
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            CursorGrabbed = true;
            _cubePrim.Camera = _camera;
            _litCubePrim.Camera = _camera;

            _litCubePrim.transform *= Matrix4.CreateRotationX(MathHelper.PiOver3) * Matrix4.CreateTranslation(2.0f, 0.0f, -1.0f);

            //GL.GetInteger(GetPName.MaxVertexAttribs, out int maxAttributeCount);
            //Debug.WriteLine($"Max vertex attributes: {maxAttributeCount}");
        }

        protected override void OnMouseWheel(MouseWheelEventArgs args)
        {
            base.OnMouseWheel(args);

            _camera.Fov -= args.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs args)
        {
            base.OnResize(args);
            GL.Viewport(0, 0, Size.X, Size.Y);

            _camera.AspectRatio = Size.X / (float)Size.Y;
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _renderTime += args.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _cubePrim.Draw();
            _litCubePrim.Draw();

            GL.BindVertexArray(_vertexArrayObject);

            var transform = Matrix4.Identity;
            transform *= Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_renderTime * 5));

            _texture.Use(TextureUnit.Texture0);
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();
            // Applying model and camera transform
            _shader.SetMatrix4("transform", transform);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }
    }
}
