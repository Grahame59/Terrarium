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

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
             _terrainGeneration = new TerrainGeneration();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            
            _shaderProgram = new ShaderProgram("vertex_shader.glsl", "fragment_shader.glsl");
            _shaderProgram.Use();
            
            _vertexArrayId = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayId);
            
            _vertexBufferId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);

            // Generate terrain data
            _terrainGeneration.GenerateTerrain(800, 600);
            
            // Upload terrain data to the GPU
            GL.BufferData(BufferTarget.ArrayBuffer, _terrainGeneration.Vertices.Length * sizeof(float),
                _terrainGeneration.Vertices, BufferUsageHint.StaticDraw);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            _shaderProgram.Use();
            GL.BindVertexArray(_vertexArrayId);
            GL.DrawArrays(PrimitiveType.Points, 0, _terrainGeneration.Vertices.Length / 3);
            
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            // Handle game updates (e.g., input processing, game logic)
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
                Title = "3D Terrain",
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
