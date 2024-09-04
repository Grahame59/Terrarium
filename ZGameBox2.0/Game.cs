using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using Error;

class Program : GameWindow
{
    private TerrainGeneration? _terrainGeneration;
    private Camera _camera;

    public Program()
        : base(GameWindowSettings.Default, new NativeWindowSettings() 
        { 
            ClientSize = new Vector2i(800, 600), 
            Title = "TERRARIUM Testing 0.3" 
        })
    {
        // Initialize _camera with a placeholder value
        _camera = new Camera(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, 1.0f);
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

        _terrainGeneration = new TerrainGeneration(_camera);

    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
         if (_terrainGeneration != null)
        {
            _terrainGeneration.Render3DCube();
        } else
        {
            ErrorLogger.SendError("TerrainGeneration Instance Field is null!", "Game.cs (Terrarium)", "NetworkListener");
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
