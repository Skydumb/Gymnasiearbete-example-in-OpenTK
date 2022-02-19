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
    class Model
    {
        protected int _vboHandle;
        protected int _vaoHandle;
        protected int _eboHandle;

        protected readonly float[] _vertices;
        protected readonly uint[] _indices;
        protected Camera _camera;
        protected Shader _shader;
        public Matrix4 transform;

        public Camera Camera { set => _camera = value; }
        public virtual Shader Shader
        {
            set
            {
                _shader = value;
                _shader.Use();
            }
        }

        public Model(float[] vertices, uint[] indices)
        {
            _vertices = vertices;
            _indices = indices;
            transform = Matrix4.Identity;
            Initialize();
        }
        protected virtual void Initialize()
        {

            _vboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(_vaoHandle);
            // Position pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _eboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        }
        public virtual void Draw()
        {
            GL.BindVertexArray(_vaoHandle);
            _shader.Use();
            // Applying model and camera transform
            _shader.SetMatrix4("transform", transform);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            // Calling Draw
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public static Model CubePrimitive()
        {
            float[] vertices = new float[]
            {
                0.5f, 0.5f, 0.5f, // Top-close-right
                0.5f, -0.5f, 0.5f, // Bottom-close-right
                -0.5f, -0.5f, 0.5f, // Bottom-close-left
                -0.5f, 0.5f, 0.5f, // Top-close-left
                -0.5f, 0.5f, -0.5f, // Top-far-left
                -0.5f, -0.5f, -0.5f, // Bottom-far-left
                0.5f, -0.5f, -0.5f, // Bottom-far-right
                0.5f, 0.5f, -0.5f, // Top-far-right
            };
            uint[] indices = new uint[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 7,
                4, 5, 6,
                4, 6, 7,
                4, 3, 2,
                4, 2, 5,
                6, 1, 2,
                6, 2, 5,
                6, 1, 0,
                6, 0, 7,
            };
            Model cube = new Model(vertices, indices);
            cube.Shader = new Shader("Shaders/Shader.vert", "Shaders/DefaultShader.frag");
            return cube;
        }
        protected static float[] FloatVAOMerge(float[] a, int aStride, float[] b, int bStride)
        {
            float[] vao = new float[a.Length + b.Length];
            var stride = aStride + bStride;
            var vaoLength = a.Length / (float)aStride;
            for (int i = 0; i < vaoLength; i++)
            {
                for (int j = 0; j < aStride; j++)
                {
                    vao[i * stride + j] = a[aStride * i + j];
                }
                for (int j = 0; j < bStride; j++)
                {
                    vao[i * stride + j + aStride] = b[i * bStride + j];
                }
            }
            return vao;
        }
    }

    class TexturedModel : Model
    {
        private Texture _texture;
        private Texture _texture1;
        protected readonly float[] _texCoords;
        public TexturedModel(float[] vertices, uint[] indices, float[] texCoords) : base(vertices, indices)
        {

        }
        public Texture Texture0
        {
            get => _texture;
            set
            {
                _texture = value;
                _texture.Use(TextureUnit.Texture0);
            }
        }
        public Texture Texture1
        {
            get => _texture1;
            set
            {
                _texture1 = value;
                _texture.Use(TextureUnit.Texture1);
            }
        }
    }

    class IlluminatedModel : Model
    {
        public IlluminatedModel() : base(null, null)
        {

        }
    }
}
