using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using Error;

namespace Terrarium
{
    public class Game : GameWindow
    {
        private ShaderProgram _shaderProgram;
        private int _vertexArrayId;
        private int _vertexBufferId;
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
            GL.ClearColor(Color4.Transparent);
            GL.Enable(EnableCap.DepthTest);

            try
            {
                _shaderProgram = new ShaderProgram(@"E:\CODES\Terrarium\Terrarium\GameBox\Shaders\vertex_shader.glsl", @"E:\CODES\Terrarium\Terrarium\GameBox\Shaders\fragment_shader.glsl");
                _shaderProgram.Use();
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Shader Program Error: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
            }

            _vertexArrayId = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayId);

            _vertexBufferId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);

            // Debug output for vertex attributes
            ErrorLogger.SendError("Vertex Array and Buffer Setup Completed", "Game.cs (Terrarium)", "NetworkListener");

            // Generate terrain data
            _terrainGeneration.GenerateTerrain(5, 5); 

            //_terrainGeneration.GenerateSimpleMesh(); //Test for Shaders

            // Check terrain vertices
            if (_terrainGeneration.Vertices.Length == 0)
            {
                ErrorLogger.SendError("Terrain vertices are empty!", "Game.cs (Terrarium)", "NetworkListener");
                return;
            }

            // Example for logging vertex positions
            for (int i = 0; i < _terrainGeneration.Vertices.Length; i += 3)
            {
                Console.WriteLine($"Vertex {i / 3}: ({_terrainGeneration.Vertices[i]}, {_terrainGeneration.Vertices[i + 1]}, {_terrainGeneration.Vertices[i + 2]})");
            }


            // Upload terrain data to the GPU
            GL.BufferData(BufferTarget.ArrayBuffer, _terrainGeneration.Vertices.Length * sizeof(float),
                _terrainGeneration.Vertices, BufferUsageHint.StaticDraw);

            // Vertex data consists of 3 floats per vertex (for the x, y, z positions)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Initialize Camera
            _camera = new Camera(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY, ClientSize.X / (float)ClientSize.Y);

            // Update camera matrices
            UpdateCamera();

            // Debug output for terrain vertices
            ErrorLogger.SendError($"Vertices Length: {_terrainGeneration.Vertices.Length}", "Game.cs (Terrarium)", "NetworkListener");

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
            }

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projectionLoc, false, ref projection);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            try
            {
                _shaderProgram.Use();
                GL.BindVertexArray(_vertexArrayId);

                var error = GL.GetError();
                if (error != ErrorCode.NoError)
                {
                    ErrorLogger.SendError($"OpenGL Error before DrawArrays: {error}", "Game.cs (Terrarium)", "NetworkListener");
                }

                GL.DrawArrays(PrimitiveType.Triangles, 0, _terrainGeneration.Vertices.Length / 3);

                error = GL.GetError();
                if (error != ErrorCode.NoError)
                {
                    ErrorLogger.SendError($"OpenGL Error after DrawArrays: {error}", "Game.cs (Terrarium)", "NetworkListener");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Rendering Error: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
            }

            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            // Handle game updates (e.g., input processing, game logic)
            _camera.Update();
            UpdateCamera();
            base.OnUpdateFrame(args);
        }

        [STAThread]
        public static void Main()
        {
            var gameWindowSettings = new GameWindowSettings()
            {
                // No direct equivalent for IsMultiSample; you may configure anti-aliasing in other ways if needed
            };

            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(800, 600), // Use ClientSize instead of Size
                Title = "Terrarium",
                // You can configure other settings if needed
            };

            using (var game = new Game(gameWindowSettings, nativeWindowSettings))
            {
                ErrorLogger.SendError("Terrarium App Connected!", "Game.cs(Terrarium Project)", "NetworkListener");
                game.Run(); // Run the game with default settings
            }
        }
    }
}
