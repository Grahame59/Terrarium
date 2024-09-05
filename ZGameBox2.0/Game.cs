using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using Error;

class Program : GameWindow
{
    private TerrainGeneration? _terrainGeneration;
    private Chunk? _chunk;
    private Camera _camera;
    private Shader _shader;


    public Program()
        : base(GameWindowSettings.Default, new NativeWindowSettings() 
        { 
            ClientSize = new Vector2i(800, 600), 
            Title = "TERRARIUM Testing 0.3" 
        })
    {
        // Initialize _camera with a placeholder value
        _camera = new Camera(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, 1.0f);

        _shader = new Shader(@"ZGameBox2.0\Shaders\vertexShader.glsl", @"ZGameBox2.0\Shaders\fragmentShader.glsl");

        // Initialize _chunk with default values or parameters
        //_chunk = new Chunk(32, 32, 0.05f, 5.0f, _camera, _shader);

    }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Enable depth testing
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
     
        // Initialize the camera with correct aspect ratio
        float aspectRatio = ClientSize.X / (float)ClientSize.Y;
        _camera = new Camera(new Vector3(10, 10, 10), Vector3.Zero, Vector3.UnitY, aspectRatio);

        // Initialize shader
        _shader = new Shader(@"ZGameBox2.0\Shaders\vertexShader.glsl", @"ZGameBox2.0\Shaders\fragmentShader.glsl");

        _terrainGeneration = new TerrainGeneration(_camera);
        

        

        // Initialize the chunks once
        if (_chunk != null)
        {
            //_chunk.InitializeChunks(chunkWidth: 32, chunkDepth: 32, scale: 0.05f, heightMultiplier: 5.0f);
        }

    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
         if (_terrainGeneration != null)
        {
       
            // Parameters for terrain generation 
            /*
            int width = 50;
            int depth = 50;
            float scale = 0.1f;
            float heightMultiplier = 5.0f; */

            // Render terrain with parameters
            //_terrainGeneration.RenderTerrainOld(width, depth, scale, heightMultiplier);

            
            _terrainGeneration.Render3DCube();

        } else
        {
            ErrorLogger.SendError("TerrainGeneration Instance Field is null!", "Game.cs (Terrarium)", "NetworkListener");
        }

        if (_chunk != null)
        {
            // Render the terrain
            //_chunk.RenderTerrain(chunkWidth: 32, chunkDepth: 32, scale: 0.05f, heightMultiplier: 5.0f);
        }
        else
        {
           // ErrorLogger.SendError("Chunk instance is null!", "Game.cs (Terrarium)", "NetworkListener");
        }

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        // Get current keyboard state
        var keyboardState = KeyboardState;

        // Update the camera based on keyboard input
        _camera.UpdateKeyboardInput(keyboardState);

    }

    protected override void OnUnload()
    {
    }

    // Entry point for the application
    [STAThread]
    public static void Main(string[] args)
    {
        // Create a new instance of the Program class and run it
        using (var game = new Program())
        {
            game.Run();
        }
    }
}
