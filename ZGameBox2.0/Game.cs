using OpenTK.Windowing.Desktop; 
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using Error;

public class Game : GameWindow
{
    
    private Shader? _shader;
    private float[,] _heightMap = new float[0, 0];
    private float[,] _normalMap = new float[0, 0];
    private uint[] _indices = new uint[0];
    private Camera? _camera;

    private List<Chunk> _chunks;
    public int frameDebugCount = 0;
    public int chunkCreationCount = 0;  // Counter for Chunk creation

    private static readonly object _lock = new object();

    public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _chunks = new List<Chunk>();
    }
    
    // Default constructor
    public Game()
        : this(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        try
        {
            ErrorLogger.SendDebug("Loading shader sources from files.", "Game.cs (Terrarium)", "NetworkListener");
            _shader = new Shader(@"ZGameBox2.0\Shaders\vertexShader.glsl", @"ZGameBox2.0\Shaders\fragmentShader.glsl");

            // Use ClientSize to get the dimensions
            float aspectRatio = ClientSize.X / (float)ClientSize.Y;
            _camera = new Camera(new Vector3(0, 10, 10), Vector3.Zero, Vector3.UnitY, aspectRatio);

            if (_chunks.Count == 0) // Ensure chunks are only initialized once
            {
                InitializeChunks();
            }
            GL.ClearColor(Color4.CornflowerBlue);

            ErrorLogger.SendDebug("Initialization completed successfully.", "Game.cs (Terrarium)", "NetworkListener");
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Initialization exception: {ex.Message}", "Game.cs", "NetworkListener");
        }
    }

    private void InitializeChunks()
    {
        try
        {
            if (_chunks.Count > 0)
            {
                ErrorLogger.SendError("Chunks already initialized.", "Game.cs", "NetworkListener");
                return;
            }

            int chunkWidth = 16;
            int chunkDepth = 16;
            float scale = 0.1f;
            float heightMultiplier = 10.0f;

            if (_camera == null || _shader == null)
            {
                ErrorLogger.SendError("Camera or Shader is not initialized.", "Game.cs", "NetworkListener");
                return;
            }

            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    _chunks.Add(new Chunk(chunkWidth, chunkDepth, scale, heightMultiplier, _camera, _shader));
                    chunkCreationCount++;  // Increment the count on each Chunk creation
                    ErrorLogger.SendDebug($"Chunk created. Total chunks: {chunkCreationCount}", "Game.cs (Terrarium)", "NetworkListener");
                }
            }
            ErrorLogger.SendDebug("Chunks initialized successfully.", "Game.cs (Terrarium)", "NetworkListener");
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Chunk initialization exception: {ex.Message}", "Game.cs", "NetworkListener");
        }
    }


    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        // Check if _camera is initialized
        if (_camera == null)
        {
            ErrorLogger.SendError("Camera or Shader is not initialized.", "Game.cs", "NetworkListener");
            return;
        }
        try
        {
            _camera.UpdateKeyboardInput(KeyboardState);
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Update frame exception: {ex.Message}", "Game.cs", "NetworkListener");
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Check if _camera and _shader are initialized
        if (_camera == null || _shader == null)
        {
            ErrorLogger.SendError("Camera or Shader is not initialized.", "Game.cs", "NetworkListener");
            return;
        }

        try
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            
            // Set global uniforms
            _shader.SetVector3("lightPos", new Vector3(10.0f, 10.0f, 10.0f));
            _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetVector3("objectColor", new Vector3(0.5f, 0.35f, 0.25f));
            _shader.SetVector3("viewPos", _camera.Position);  // Add this line to set viewPos

            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            foreach (var chunk in _chunks)
            {
                ErrorLogger.SendDebug("Rendering chunk.", "Game.cs (Terrarium)", "NetworkListener");
                chunk.Render();
            }

            SwapBuffers();

            lock (_lock)
            {
                if (frameDebugCount == 0)
                {
                    ErrorLogger.SendDebug("Frame rendered successfully.", "Game.cs (Terrarium)", "NetworkListener");
                    frameDebugCount++;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Render frame exception: {ex.Message}", "Game.cs", "NetworkListener");
        }
    }
    protected override void OnUnload()
    {
        // Implement cleanup if necessary
    }

    [STAThread]
    public static void Main(string[] args)
    {
        using (var game = new Game())
        {
            game.Run();
        }
    }
}
