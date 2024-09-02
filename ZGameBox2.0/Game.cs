using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using Error;

class Program : GameWindow
{
    private int _vertexArrayObject;
    private int _vertexBufferObject;
    private int _indexBufferObject;
    private Shader? _shader;
    private TerrainGeneration _terrainGeneration;
    private TerrainGeneration.Mesh _terrainMesh;

    public Program()
        : base(GameWindowSettings.Default, new NativeWindowSettings() 
        { 
            ClientSize = new Vector2i(800, 600), 
            Title = "TERRARIUM 2.0" 
        })
    {
        _terrainGeneration = new TerrainGeneration();
        // Generate a heightmap and mesh
        float[,] heightmap = _terrainGeneration.GenerateHeightmap(256, 256, 0.1f);
        _terrainMesh = _terrainGeneration.GenerateMesh(heightmap);
    }

    protected override void OnLoad()
    {
        try
        {
            base.OnLoad();

            // Create and bind VAO
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            // Create and bind VBO
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _terrainMesh.vertices.Length * Vector3.SizeInBytes, _terrainMesh.vertices, BufferUsageHint.StaticDraw);

            // Create and bind IBO
            _indexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _terrainMesh.triangles.Length * sizeof(int), _terrainMesh.triangles, BufferUsageHint.StaticDraw);

            // Set vertex attributes
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);

            // Load and compile shaders
            _shader = new Shader(@"E:\CODES\Terrarium\Terrarium\ZGameBox2.0\Shaders\vertexShader.glsl", @"E:\CODES\Terrarium\Terrarium\ZGameBox2.0\Shaders\fragmentShader.glsl");
            _shader.Use();

            // Check if shader program is active
            int currentProgram;
            GL.GetInteger(GetPName.CurrentProgram, out currentProgram);
            if (currentProgram != _shader.Handle)
            {
                Console.WriteLine("Error: Shader program not active.");
            }

            // Check for OpenGL errors
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine("OpenGL Error during OnLoad: " + error);
            }

            // Set the background color
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Error in OnLoad: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        try
        {
            base.OnRenderFrame(e);

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Use the shader and bind VAO
            if (_shader != null)
            {
                _shader.Use();
                GL.BindVertexArray(_vertexArrayObject);

                // Draw elements
                GL.DrawElements(PrimitiveType.Triangles, _terrainMesh.triangles.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

                // Check for OpenGL errors
                ErrorCode error = GL.GetError();
                if (error != ErrorCode.NoError)
                {
                    Console.WriteLine("OpenGL Error during OnRenderFrame: " + error);
                }
            }
            else
            {
                ErrorLogger.SendError("Shader is not initialized.", "Game.cs (Terrarium)", "NetworkListener");
            }

            // Swap buffers
            SwapBuffers();
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Error in OnRenderFrame: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
        }
    }


    protected override void OnUnload()
    {
        try
        {
            base.OnUnload();

            // Clean up all resources
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vertexBufferObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(_indexBufferObject);

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_vertexArrayObject);

            if (_shader != null)
            {
                GL.UseProgram(0);
                GL.DeleteProgram(_shader.Handle);
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Error in OnUnload: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
        }
    }

    static void Main(string[] args)
    {
        try
        {
            using (var program = new Program())
            {
                program.Run();
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Error in Main: {ex.Message}", "Game.cs (Terrarium)", "NetworkListener");
        }
    }
}
