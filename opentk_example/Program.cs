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
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };
        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;
        private int _elementBufferObject;
        private Texture _texture;
        private Texture _texture2;

        //private Stopwatch _timer;

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
            if (KeyboardState.IsKeyDown(Keys.Escape)) this.Close();
            base.OnUpdateFrame(args);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(1f, 1f, 1f, 1f);
            _vertexBufferObject = GL.GenBuffer();
            // Binding VBO
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

            _texture = Texture.LoadFromFile("Resources/hamster.png");
            _texture.Use(TextureUnit.Texture0);
            _texture2 = Texture.LoadFromFile("Resources/container.png");
            _texture2.Use(TextureUnit.Texture1);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // Adding Shader
            _shader = new Shader("Shaders/Shader.vert", "Shaders/Shader.frag");
            _shader.Use();

            _shader.SetInt("texture0", 0);
            _shader.SetInt("texture1", 1);
            //GL.GetInteger(GetPName.MaxVertexAttribs, out int maxAttributeCount);
            //Debug.WriteLine($"Max vertex attributes: {maxAttributeCount}");
            //_timer = new Stopwatch();
            //_timer.Start();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _texture.Use(TextureUnit.Texture0);
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }
    }
}
