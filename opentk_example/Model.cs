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
        public virtual void Draw(bool final = true)
        {
            GL.BindVertexArray(_vaoHandle);
            _shader.Use();
            // Applying model and camera transform
            _shader.SetMatrix4("transform", transform);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            // Calling Draw
            if (final) GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
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
            cube.Initialize();
            return cube;
        }
        public static IlluminatedModel SharpCubePrimitve()
        {
            float[] vertices = new float[]
            {
                // Front
                0.5f, 0.5f, 0.5f, // Top-close-right
                0.5f, -0.5f, 0.5f, // Bottom-close-right
                -0.5f, -0.5f, 0.5f, // Bottom-close-left
                -0.5f, 0.5f, 0.5f, // Top-close-left
                // Back
                -0.5f, 0.5f, -0.5f, // Top-far-left
                -0.5f, -0.5f, -0.5f, // Bottom-far-left
                0.5f, -0.5f, -0.5f, // Bottom-far-right
                0.5f, 0.5f, -0.5f, // Top-far-right
                // Top
                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, 0.5f,
                // Bottom
                0.5f, -0.5f, -0.5f, // Far-right
                0.5f, -0.5f, 0.5f, // Close-right
                -0.5f, -0.5f, 0.5f, // Close-left
                -0.5f, -0.5f, -0.5f, // Far-Left
                // Left
                -0.5f, -0.5f, -0.5f, // Bottom-left
                -0.5f, -0.5f, 0.5f, // Bottom-right
                -0.5f, 0.5f, 0.5f, // Top-right
                -0.5f, 0.5f, -0.5f, // Top-Left
                // Right
                0.5f, -0.5f, 0.5f, // Bottom-left
                0.5f, -0.5f, -0.5f, // Bottom-right
                0.5f, 0.5f, -0.5f, // Top-right
                0.5f, 0.5f, 0.5f, // Top-left
            };
            float[] normals = new float[]
            {
                // Front
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                // Back
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                // Top
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                // Bottom
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                // Left
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                // Right
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
            };
            uint[] indices = new uint[]
            {
                0, 1, 2,
                0, 2, 3,
                4, 5, 6,
                4, 6, 7,
                8, 9, 10,
                8, 10, 11,
                12, 13, 14,
                12, 14, 15,
                16, 17, 18,
                16, 18, 19,
                20, 21, 22,
                20, 22, 23,
            };
            IlluminatedModel cube = new IlluminatedModel(vertices, indices, normals);
            cube.Shader = new Shader("Shaders/Shader.vert", "Shaders/Lighting.frag");
            cube.Initialize();
            return cube;
        }
        public static TexturedModel SharpTexturedCubePrimitive()
        {
            float[] vertices = new float[]
            {
                // Front
                0.5f, 0.5f, 0.5f, // Top-close-right
                0.5f, -0.5f, 0.5f, // Bottom-close-right
                -0.5f, -0.5f, 0.5f, // Bottom-close-left
                -0.5f, 0.5f, 0.5f, // Top-close-left
                // Back
                -0.5f, 0.5f, -0.5f, // Top-far-left
                -0.5f, -0.5f, -0.5f, // Bottom-far-left
                0.5f, -0.5f, -0.5f, // Bottom-far-right
                0.5f, 0.5f, -0.5f, // Top-far-right
                // Top
                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, 0.5f,
                // Bottom
                0.5f, -0.5f, -0.5f, // Far-right
                0.5f, -0.5f, 0.5f, // Close-right
                -0.5f, -0.5f, 0.5f, // Close-left
                -0.5f, -0.5f, -0.5f, // Far-Left
                // Left
                -0.5f, -0.5f, -0.5f, // Bottom-left
                -0.5f, -0.5f, 0.5f, // Bottom-right
                -0.5f, 0.5f, 0.5f, // Top-right
                -0.5f, 0.5f, -0.5f, // Top-Left
                // Right
                0.5f, -0.5f, 0.5f, // Bottom-left
                0.5f, -0.5f, -0.5f, // Bottom-right
                0.5f, 0.5f, -0.5f, // Top-right
                0.5f, 0.5f, 0.5f, // Top-left
            };
            float[] normals = new float[]
            {
                // Front
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                // Back
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                // Top
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                // Bottom
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                // Left
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                // Right
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
            };
            uint[] indices = new uint[]
            {
                0, 1, 2,
                0, 2, 3,
                4, 5, 6,
                4, 6, 7,
                8, 9, 10,
                8, 10, 11,
                12, 13, 14,
                12, 14, 15,
                16, 17, 18,
                16, 18, 19,
                20, 21, 22,
                20, 22, 23,
            };
            float[] texCoords = new float[]
            {
                1.0f, 1.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f,

                1.0f, 1.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f,

                1.0f, 1.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f,

                1.0f, 1.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f,

                1.0f, 1.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f,

                1.0f, 1.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f,
            };
            TexturedModel cube = new TexturedModel(vertices, indices, texCoords, normals)
            {
                Shader = new Shader("Shaders/Shader.vert", "Shaders/Mapping.frag"),
                Texture = Texture.LoadFromFile("Resources/container.png"),
                SpecularMap = Texture.LoadFromFile("Resources/container.png")
            };
            cube.Initialize();
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
        private Texture _specMap;
        private readonly float[] _vao;
        public Vector3 lightPosition;
        public TexturedModel(float[] vertices, uint[] indices, float[] texCoords, float[] normals) : base(vertices, indices)
        {
            float[] vao = FloatVAOMerge(vertices, 3, normals, 3);
            _vao = FloatVAOMerge(vao, 6, texCoords, 2);
        }
        public Texture Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                _texture.Use(TextureUnit.Texture0);
            }
        }
        public Texture SpecularMap
        {
            get => _specMap;
            set
            {
                _specMap = value;
                _specMap.Use(TextureUnit.Texture1);
            }
        }
        protected override void Initialize()
        {
            _vboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, _vao.Length * sizeof(float), _vao, BufferUsageHint.StaticDraw);

            _vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(_vaoHandle);
            // Position pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // Normal pointers
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            // Texture pointers
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            _eboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        }
        public override void Draw(bool final = true)
        {
            base.Draw(false);
            _shader.SetVector3("viewPos", _camera.Position);

            _shader.SetInt("material.diffuse", 0);
            _shader.SetInt("material.specular", 1);
            _shader.SetFloat("material.shininess", 32f);

            _shader.SetVector3("light.direction", lightPosition);
            _shader.SetVector3("light.ambient", new Vector3(0.2f));
            _shader.SetVector3("light.diffuse", new Vector3(0.5f));
            _shader.SetVector3("light.specular", new Vector3(1.0f, 1.0f, 1.0f));

            _texture.Use(TextureUnit.Texture0);
            _specMap.Use(TextureUnit.Texture1);

            if (final) GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }

    class IlluminatedModel : Model
    {
        private readonly float[] _vao;
        public Vector3 lightPositon;
        public IlluminatedModel(float[] vertices, uint[] indices, float[] normals) : base(vertices, indices)
        {
            _vao = FloatVAOMerge(_vertices, 3, normals, 3);
        }
        protected override void Initialize()
        {
            _vboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, _vao.Length * sizeof(float), _vao, BufferUsageHint.StaticDraw);

            _vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(_vaoHandle);
            // Position pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // Normal pointers
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            _eboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        }
        public override void Draw(bool final = true)
        {
            base.Draw(false);
            _shader.SetVector3("viewPos", _camera.Position);

            _shader.SetVector3("material.ambient", new Vector3(1.5f, 0.5f, 0.31f));
            _shader.SetVector3("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
            _shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _shader.SetFloat("material.shininess", 32f);

            _shader.SetVector3("light.position", lightPositon);
            _shader.SetVector3("light.ambient", new Vector3(0.2f));
            _shader.SetVector3("light.diffuse", new Vector3(0.5f));
            _shader.SetVector3("light.specular", new Vector3(1.0f, 1.0f, 1.0f));

            if (final) GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
