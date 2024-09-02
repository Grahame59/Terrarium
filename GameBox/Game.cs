using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using Error;

/*

namespace Terrarium
{
    public class Game : GameWindow
    {
        private ShaderProgram _shaderProgram;
        private int _vertexArrayId;
        private int _vertexBufferId;
        private int _elementBufferId;
        private TerrainGeneration _terrainGeneration;
        private Camera _camera;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _terrainGeneration = new TerrainGeneration();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);

            try
            {
                _shaderProgram = new ShaderProgram(@"E:\CODES\Terrarium\Terrarium\GameBox\Shaders\vertex_shader.glsl", @"E:\CODES\Terrarium\Terrarium\GameBox\Shaders\fragment_shader.glsl");
                _shaderProgram.Use();
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Shader Program Error: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
                return;
            }

            _terrainGeneration.GenerateTerrain(100, 100, 1f, 10f);

            InitializeBuffers();

            if (_terrainGeneration.Vertices.Length == 0)
            {
                ErrorLogger.SendError("Terrain vertices are empty!", "Game.cs (Terrarium)", "NetworkListener");
                return;
            }

            InitializeCamera();

            ErrorLogger.SendError($"Vertices Length: {_terrainGeneration.Vertices.Length}", "Game.cs (Terrarium)", "NetworkListener");
        }

        private void InitializeBuffers()
        {
            try
            {
                _vertexArrayId = GL.GenVertexArray();
                GL.BindVertexArray(_vertexArrayId);

                _vertexBufferId = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
                GL.BufferData(BufferTarget.ArrayBuffer, _terrainGeneration.Vertices.Length * sizeof(float), _terrainGeneration.Vertices, BufferUsageHint.StaticDraw);
                
                // Check for errors after buffer data upload
                ErrorCode error = GL.GetError();
                if (error != ErrorCode.NoError)
                {
                    ErrorLogger.SendError($"OpenGL Error after BufferData: {error}", "Game.cs (Terrarium)", "NetworkListener");
                }
                
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                _elementBufferId = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferId);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _terrainGeneration.Indices.Length * sizeof(uint), _terrainGeneration.Indices, BufferUsageHint.StaticDraw);

                // Check for errors after element buffer data upload
                error = GL.GetError();
                if (error != ErrorCode.NoError)
                {
                    ErrorLogger.SendError($"OpenGL Error after ElementBufferData: {error}", "Game.cs (Terrarium)", "NetworkListener");
                }

                GL.BindVertexArray(0);
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Buffer Initialization Error: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
                throw;
            }
        }

        private void LogBufferData()
        {
            // Bind the vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);

            // Map the buffer for reading
            float[] bufferData = new float[_terrainGeneration.Vertices.Length];
            GL.GetBufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _terrainGeneration.Vertices.Length * sizeof(float), bufferData);

            // Log the buffer data
            for (int i = 0; i < Math.Min(bufferData.Length, 30); i += 3)
            {
                ErrorLogger.SendError($"Buffer Data {i / 3}: ({bufferData[i]}, {bufferData[i + 1]}, {bufferData[i + 2]})", "Game.cs (Terrarium)", "NetworkListener");
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            try
            {
                _shaderProgram.Use();
                GL.BindVertexArray(_vertexArrayId);

                // Check for errors before drawing
                ErrorCode error = GL.GetError();
                if (error != ErrorCode.NoError)
                {
                    ErrorLogger.SendError($"OpenGL Error before DrawElements: {error}", "Game.cs (Terrarium)", "NetworkListener");
                }

                GL.DrawElements(PrimitiveType.Triangles, _terrainGeneration.Indices.Length, DrawElementsType.UnsignedInt, 0);

                // Check for errors after drawing
                error = GL.GetError();
                if (error != ErrorCode.NoError)
                {
                    ErrorLogger.SendError($"OpenGL Error after DrawElements: {error}", "Game.cs (Terrarium)", "NetworkListener");
                }
                else
                {
                    ErrorLogger.SendError($"Drew {_terrainGeneration.Vertices.Length / 3} vertices", "Game.cs (Terrarium)", "NetworkListener");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Rendering Error: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
            }

            SwapBuffers();
        }


        private void InitializeCamera()
        {
            _camera = new Camera(new Vector3(25, 15, 25), new Vector3(25, 0, 25), Vector3.UnitY, ClientSize.X / (float)ClientSize.Y);
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            var model = Matrix4.Identity;
            var view = _camera.View;
            var projection = _camera.Projection;

            int modelLoc = GL.GetUniformLocation(_shaderProgram.Handle, "model");
            int viewLoc = GL.GetUniformLocation(_shaderProgram.Handle, "view");
            int projectionLoc = GL.GetUniformLocation(_shaderProgram.Handle, "projection");

            if (modelLoc == -1 || viewLoc == -1 || projectionLoc == -1)
            {
                ErrorLogger.SendError("Uniform location error!", "Game.cs (Terrarium)", "NetworkListener");
                return;
            }

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projectionLoc, false, ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            _camera.Update();
            UpdateCamera();
            base.OnUpdateFrame(args);
        }

        [STAThread]
        public static void Main()
        {
            var gameWindowSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings
            {
                ClientSize = new Vector2i(800, 600),
                Title = "Terrarium",
            };

            using (var game = new Game(gameWindowSettings, nativeWindowSettings))
            {
                ErrorLogger.SendError("Terrarium App Connected!", "Game.cs (Terrarium)", "NetworkListener");
                game.Run();
            }
        } 
    } 
}
*/
