using System;
using System.Diagnostics; // For Stopwatch
using SkiaSharp;
using OpenTK.Mathematics;
using Error; // Custom error handling
using Terrarium; // Custom terrain generation
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Security.Cryptography.X509Certificates;

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
            
            _camera = camera ?? throw new ArgumentNullException(nameof(camera), "Camera cannot be null.");

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
            _shader.SetVector3("lightPos", new Vector3(10.0f, 10.0f, 10.0f));
            _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetVector3("objectColor", new Vector3(0.5f, 0.35f, 0.25f)); // Example color
            _shader.Use();
            
            if (_camera != null)
            {
                // Set uniforms
                var model = Matrix4.Identity; // or update based on cube's transformation
                var view = _camera.View;
                var projection = _camera.Projection;

                _shader?.SetMatrix4("model", Matrix4.Identity);
                _shader?.SetMatrix4("view", _camera.View);
                _shader?.SetMatrix4("projection", _camera.Projection);
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
        

    public void RenderTerrainOld(int width, int depth, float scale, float heightMultiplier)
    {
        try
        {
            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Use shader program
            _shader.SetVector3("lightPos", new Vector3(10.0f, 10.0f, 10.0f));
            _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetVector3("objectColor", new Vector3(0.5f, 0.35f, 0.25f)); // Example color
            _shader.Use();
            
                
            if (_camera != null)
            {
                // Set camera matrices
                _shader.SetVector3("viewPos", _camera.Position);
                _shader.SetMatrix4("view", _camera.View);
                _shader.SetMatrix4("projection", _camera.Projection);

                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        // Generate height using Perlin noise
                        float height = Mathf.PerlinNoise(x * scale, 0, z * scale) * heightMultiplier;

                        // Model matrix for positioning each cube
                        var model = Matrix4.CreateTranslation(x, height, z);
                        _shader.SetMatrix4("model", model);

                        // Bind the cube's VAO
                        GL.BindVertexArray(_cubeVertexArrayObject);

                        // Draw the cube
                        GL.DrawElements(PrimitiveType.Triangles, _cubeIndices.Length, DrawElementsType.UnsignedInt, 0);
                    }
                }
            }
            else
            {
                ErrorLogger.SendError("Camera instance is null!", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Terrain rendering exception: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
    }


} //end of terrainGeneration class

//----------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------



//----------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------
public class Chunk
{
    private readonly int _width;
    private readonly int _depth;
    private readonly float _scale;
    private readonly float _heightMultiplier;
    private float[,] _heightMap;
    private readonly int _vertexCount;
    private readonly uint[] _indices;
    private readonly int _vertexArrayObject;
    private readonly int _vertexBufferObject;
    private readonly int _elementBufferObject;


    private List<Chunk> _chunks = new List<Chunk>();
     private readonly Camera _camera;
    private readonly Shader _shader;


    public Chunk(int width, int depth, float scale, float heightMultiplier, Camera camera, Shader shader)
    {
        _width = width;
        _depth = depth;
        _scale = scale;
        _heightMultiplier = heightMultiplier;
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
        _shader = shader ?? throw new ArgumentNullException(nameof(shader));


        _vertexCount = (width + 1) * (depth + 1);
        _indices = GenerateIndices(width, depth);
        
        _vertexArrayObject = GL.GenVertexArray();
        _vertexBufferObject = GL.GenBuffer();
        _elementBufferObject = GL.GenBuffer();

        // Initialize _heightMap
        _heightMap = new float[width + 1, depth + 1];

        GenerateTerrain();
        SetupBuffers();
    }

    private void GenerateTerrain()
    {
        _heightMap = new float[_width, _depth];
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _depth; z++)
            {
                _heightMap[x, z] = Mathf.PerlinNoise(x * _scale, 0, z * _scale) * _heightMultiplier;
            }
        }
    }

    private void SetupBuffers()
    {
        GL.BindVertexArray(_vertexArrayObject);

        // Vertex buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertexCount * 3 * sizeof(float), GenerateVertices(), BufferUsageHint.StaticDraw);

        // Element buffer
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        // Setup vertex attributes
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    private float[] GenerateVertices()
    {
        var vertices = new List<float>();

        for (int x = 0; x < _width; x++)  // Changed from x <= _width
        {
            for (int z = 0; z < _depth; z++)  // Changed from z <= _depth
            {
                vertices.Add(x);
                vertices.Add(_heightMap[x, z]);
                vertices.Add(z);
            }
        }

        return vertices.ToArray();
    }


    private uint[] GenerateIndices(int width, int depth)
    {
        var indices = new List<uint>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                int topLeft = x + z * (width + 1);
                int topRight = (x + 1) + z * (width + 1);
                int bottomLeft = x + (z + 1) * (width + 1);
                int bottomRight = (x + 1) + (z + 1) * (width + 1);

                indices.Add((uint)topLeft);
                indices.Add((uint)bottomLeft);
                indices.Add((uint)topRight);
                indices.Add((uint)topRight);
                indices.Add((uint)bottomLeft);
                indices.Add((uint)bottomRight);
            }
        }

        return indices.ToArray();
    }

    public void Render()
    {
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void RenderTerrain(int chunkWidth, int chunkDepth, float scale, float heightMultiplier)
    {
        try
        {
            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Use shader program
            if (_shader == null)
            {
                ErrorLogger.SendError("Shader is not initialized.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
                throw new InvalidOperationException("Shader is not initialized.");
            }


            // Use shader program
            _shader.Use();
            _shader.SetVector3("lightPos", new Vector3(10.0f, 10.0f, 10.0f));
            _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetVector3("objectColor", new Vector3(0.5f, 0.35f, 0.25f));

             if (_camera == null)
            {
                ErrorLogger.SendError("Camera is not initialized.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
                throw new InvalidOperationException("Camera is not initialized.");
            }

            // Set camera matrices
            _shader.SetVector3("viewPos", _camera.Position);
            _shader.SetMatrix4("view", _camera.View);
            _shader.SetMatrix4("projection", _camera.Projection);

            foreach (var chunk in _chunks)
            {
                if (_chunks == null)
                {
                    ErrorLogger.SendError("Chunks list is not initialized.", "TerrainGenerator.cs (Terrarium)", "NetworkListener");
                    throw new InvalidOperationException("Chunks list is not initialized.");
                }
                chunk.Render();
            }
            
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Terrain rendering exception: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
    }

    public void InitializeChunks(int chunkWidth, int chunkDepth, float scale, float heightMultiplier)
    {
        _chunks.Clear();

        int numChunksX = 3; // Number of chunks in the x direction
        int numChunksZ = 3; // Number of chunks in the z direction

        for (int x = 0; x < numChunksX; x++)
        {
            for (int z = 0; z < numChunksZ; z++)
            {
                _chunks.Add(new Chunk(chunkWidth, chunkDepth, scale, heightMultiplier, _camera, _shader));
            }
        }
    }

}

