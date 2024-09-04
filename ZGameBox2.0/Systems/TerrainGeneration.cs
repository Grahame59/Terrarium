using System;
using System.Diagnostics; // For Stopwatch
using SkiaSharp;
using OpenTK.Mathematics;
using Error; // Custom error handling
using Terrarium; // Custom terrain generation
using OpenTK.Graphics.OpenGL4;
using System.IO;

public class TerrainGeneration
{
    //Triangle vars
    private int _triangleVertexArrayObject;
    private int _triangleVertexBufferObject;
    public int shaderTriangleDebug = 0;

    //Cube vars
    private int _cubeVertexArrayObject;
    private int _cubeVertexBufferObject;
    private int _cubeElementBufferObject;

    //shader field Instance
    private Shader _shader = null!;
    private readonly Camera? _camera;
    

    private readonly float[] _cubeVertices = {
        // Positions         
        -0.5f, -0.5f, -0.5f, // Front-bottom-left
         0.5f, -0.5f, -0.5f, // Front-bottom-right
         0.5f,  0.5f, -0.5f, // Front-top-right
        -0.5f,  0.5f, -0.5f, // Front-top-left
        -0.5f, -0.5f,  0.5f, // Back-bottom-left
         0.5f, -0.5f,  0.5f, // Back-bottom-right
         0.5f,  0.5f,  0.5f, // Back-top-right
        -0.5f,  0.5f,  0.5f  // Back-top-left
    };

    private readonly uint[] _cubeIndices = {
        // Front face
        0, 1, 2, 2, 3, 0,
        // Back face
        4, 5, 6, 6, 7, 4,
        // Left face
        0, 3, 7, 7, 4, 0,
        // Right face
        1, 2, 6, 6, 5, 1,
        // Top face
        2, 3, 7, 7, 6, 2,
        // Bottom face
        0, 1, 5, 5, 4, 0
    };

    public TerrainGeneration(Camera camera)
    {
        try
        {
            // Initialize shaders
            _shader = new Shader
            (
                @"ZGameBox2.0\Shaders\vertexShader.glsl", 
                @"ZGameBox2.0\Shaders\fragmentShader.glsl"
            );
            
            _camera = camera; // Ensure camera is passed in and initialized

            if (_shader == null!)
            {
                ErrorLogger.SendError("Shader is in null state!", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
            }
            
            // Initialize Triangle
            InitializeTriangle();

            // Initialize Cube
            InitializeCube();

            ErrorLogger.SendError("Basic Terrain generation setup completed successfully.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Initialization Exception: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
    }

    private void InitializeTriangle()
    {
        _triangleVertexArrayObject = GL.GenVertexArray();
        _triangleVertexBufferObject = GL.GenBuffer();

        GL.BindVertexArray(_triangleVertexArrayObject);

        // Simple vertex data for a single triangle
        float[] vertices = {
            0.0f,  0.5f, 0.0f,
           -0.5f, -0.5f, 0.0f,
            0.5f, -0.5f, 0.0f
        };

        GL.BindBuffer(BufferTarget.ArrayBuffer, _triangleVertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0); // Unbind VAO
    }

    private void InitializeCube()
    {
        _cubeVertexArrayObject = GL.GenVertexArray();
        _cubeVertexBufferObject = GL.GenBuffer();
        _cubeElementBufferObject = GL.GenBuffer();

        GL.BindVertexArray(_cubeVertexArrayObject);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _cubeVertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _cubeVertices.Length * sizeof(float), _cubeVertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _cubeElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _cubeIndices.Length * sizeof(uint), _cubeIndices, BufferUsageHint.StaticDraw);

        GL.BindVertexArray(0); // Unbind VAO
    }

    public void RenderTriangle()
    {
        try
        {
            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Use shader program
            _shader.Use();

            // Bind the vertex array object
            GL.BindVertexArray(_triangleVertexArrayObject);

            // Draw the triangle
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            if (shaderTriangleDebug == 0)
            {
                ErrorLogger.SendError("Triangle rendered successfully.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
                shaderTriangleDebug++;
            }

        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Rendering Exception: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
    }

        public void Render3DCube()
        {
            try
            {
                // Clear the screen
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Use shader program
                _shader.Use();
                
                if (_camera != null)
                {
                    // Set uniforms
                    var model = Matrix4.Identity; // or update based on cube's transformation
                    var view = _camera.View;
                    var projection = _camera.Projection;

                    _shader.SetMatrix4("model", Matrix4.Identity);
                    _shader.SetMatrix4("view", _camera.View);
                    _shader.SetMatrix4("projection", _camera.Projection);
                } else
                {
                    ErrorLogger.SendError("Camera instance is null!", "TerrainGeneration.cs", "NetworkListener");
                }
     
                // Bind the vertex array object
                GL.BindVertexArray(_cubeVertexArrayObject);

                // Draw the cube
                GL.DrawElements(PrimitiveType.Triangles, _cubeIndices.Length, DrawElementsType.UnsignedInt, 0);

                if (shaderTriangleDebug == 0)
                {
                    ErrorLogger.SendError("Cube rendered successfully.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
                    shaderTriangleDebug++;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Rendering Exception: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
            }
        }








} //end of terrainGeneration class
