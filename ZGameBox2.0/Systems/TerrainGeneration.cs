using System;
using System.Diagnostics; // For Stopwatch
using SkiaSharp;
using OpenTK.Mathematics;
using Error; // Custom error handling
using Terrarium; // Custom terrain generation
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;

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
                        //float height = Mathf.PerlinNoise(x * scale, 0, z * scale) * heightMultiplier;

                        // Model matrix for positioning each cube
                       // var model = Matrix4.CreateTranslation(x, height, z);
                        //_shader.SetMatrix4("model", model);

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
    private float[,] _heightMap = new float[0, 0]; // Initialize with default empty array
    private Vector3[,] _normalMap = new Vector3[0, 0]; // Initialize with default empty array
    private readonly int _vertexCount;
    private uint[] _indices = new uint[0]; // Initialize with default empty array
    private readonly int _vertexArrayObject;
    private readonly int _vertexBufferObject;
    private readonly int _normalBufferObject;
    private readonly int _elementBufferObject;
    private readonly Camera? _camera;
    private readonly Shader? _shader;
    public int bufferDebugCount = 0;
    public int terrainGenDebugCount = 0;
    public int renderDebugCount = 0;
    private static readonly object _lock = new object();
    private bool _initialized = false;
    private bool _terrainGenerated = false;
    private bool _buffersSetUp = false;

    public Chunk(int width, int depth, float scale, float heightMultiplier, Camera camera, Shader shader)
    {
        if (_initialized)
        {
            ErrorLogger.SendError("Chunk already initialized.", "Chunk.cs (Terrarium)", "NetworkListener");
            return;
        }

        _initialized = true;

        _width = width;
        _depth = depth;
        _scale = scale;
        _heightMultiplier = heightMultiplier;
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
        _shader = shader ?? throw new ArgumentNullException(nameof(shader));

        ErrorLogger.SendDebug($"Chunk constructor called at {DateTime.Now}", "Chunk.cs (Terrarium)", "NetworkListener");
        _vertexCount = (_width + 1) * (_depth + 1);
        _indices = GenerateIndices();

        _vertexArrayObject = GL.GenVertexArray();
        _vertexBufferObject = GL.GenBuffer();
        _normalBufferObject = GL.GenBuffer();
        _elementBufferObject = GL.GenBuffer();

        _heightMap = new float[_width + 1, _depth + 1];
        _normalMap = new Vector3[_width + 1, _depth + 1];
        GenerateTerrain();
        SetupBuffers();
    }

    private void GenerateTerrain()
    {
        lock (_lock)
        {
            if (_terrainGenerated)
            {
                ErrorLogger.SendError("Terrain already generated.", "Chunk.cs (Terrarium)", "NetworkListener");
                return;
            }

            _terrainGenerated = true;

            ErrorLogger.SendDebug("Generating terrain...", "Chunk.cs (Terrarium)", "NetworkListener");

            // Generate height map
            for (int x = 0; x <= _width; x++)
            {
                for (int z = 0; z <= _depth; z++)
                {
                    _heightMap[x, z] = PerlinNoise.Get(x * _scale, 0, z * _scale) * _heightMultiplier;
                }
            }
            
            // Log height map values for debugging
            int limitdebugMax = 20;
            for (int x = 0; x <= _width; x++)
            {
                for (int z = 0; z <= _depth; z++)
                {
                    for (int limitdebug = 0; limitdebug <= limitdebugMax; limitdebug++)
                    {
                        ErrorLogger.SendDebug($"HeightMap[{x}, {z}] = {_heightMap[x, z]}", "Chunk.cs (Terrarium)", "NetworkListener");
                    }
                }
            }

            // Calculate normals
            CalculateNormals();

            lock (_lock)
            {
                if(terrainGenDebugCount == 0)
                {
                    ErrorLogger.SendDebug("Terrain and normals generated successfully.", "Chunk.cs (Terrarium)", "NetworkListener");
                    terrainGenDebugCount++;
                }
            }
        }
    }

    private void CalculateNormals()
    {
        for (int x = 0; x <= _width; x++)
        {
            for (int z = 0; z <= _depth; z++)
            {
                Vector3 normal = Vector3.Zero;
                int count = 0;

                // Calculate normal for surrounding triangles
                for (int dx = -1; dx <= 0; dx++)
                {
                    for (int dz = -1; dz <= 0; dz++)
                    {
                        if (x + dx >= 0 && x + dx < _width && z + dz >= 0 && z + dz < _depth)
                        {
                            Vector3 v1 = new Vector3(x + dx, _heightMap[x + dx, z + dz], z + dz);
                            Vector3 v2 = new Vector3(x + dx + 1, _heightMap[x + dx + 1, z + dz], z + dz);
                            Vector3 v3 = new Vector3(x + dx, _heightMap[x + dx, z + dz + 1], z + dz + 1);
                            Vector3 triangleNormal = Vector3.Cross(v2 - v1, v3 - v1);
                            normal += triangleNormal;
                            count++;
                        }
                    }
                }

                if (count > 0)
                {
                    normal /= count;
                    normal.Normalize();
                }
                else
                {
                    normal = Vector3.UnitY;
                }

                _normalMap[x, z] = normal;
            }
        }
    }

    private void SetupBuffers()
    {
        lock (_lock)
        {
            if (_buffersSetUp)
            {
                ErrorLogger.SendError("Buffers already set up.", "Chunk.cs (Terrarium)", "NetworkListener");
                return;
            }

            _buffersSetUp = true;

            GL.BindVertexArray(_vertexArrayObject);

            // Vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertexCount * 3 * sizeof(float), GenerateVertices(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Normal buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _normalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertexCount * 3 * sizeof(float), GenerateNormals(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);

            // Index buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            lock (_lock)
            {
                if (bufferDebugCount == 0)
                {
                    ErrorLogger.SendDebug("Buffers set up successfully.", "Chunk.cs (Terrarium)", "NetworkListener");
                    bufferDebugCount++;
                }
            }
        }
    }

    private float[] GenerateVertices()
    {
        var vertices = new List<float>();

        for (int z = 0; z <= _depth; z++)
        {
            for (int x = 0; x <= _width; x++)
            {
                vertices.Add(x);
                vertices.Add(_heightMap[x, z]);
                vertices.Add(z);
            }
        }

        return vertices.ToArray();
    }

    private float[] GenerateNormals()
    {
        var normals = new List<float>();

        for (int z = 0; z <= _depth; z++)
        {
            for (int x = 0; x <= _width; x++)
            {
                Vector3 normal = _normalMap[x, z];
                normals.Add(normal.X);
                normals.Add(normal.Y);
                normals.Add(normal.Z);
            }
        }

        return normals.ToArray();
    }

    private uint[] GenerateIndices()
    {
        var indices = new List<uint>();

        for (int z = 0; z < _depth; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                uint topLeft = (uint)(x + z * (_width + 1));
                uint topRight = (uint)((x + 1) + z * (_width + 1));
                uint bottomLeft = (uint)(x + (z + 1) * (_width + 1));
                uint bottomRight = (uint)((x + 1) + (z + 1) * (_width + 1));

                indices.Add(topLeft);
                indices.Add(bottomLeft);
                indices.Add(topRight);
                indices.Add(topRight);
                indices.Add(bottomLeft);
                indices.Add(bottomRight);
            }
        }

        return indices.ToArray();
    }

    public void Render()
    {
        try
        {
            // Set chunk-specific uniforms or transformations here if needed
            // For example, if each chunk has a different position:
            // _shader.SetMatrix4("model", Matrix4.CreateTranslation(ChunkPosition));

            GL.BindVertexArray(_vertexArrayObject);
            ErrorLogger.SendDebug("Vertex Array Object bound.", "Chunk.cs (Terrarium)", "NetworkListener");

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            ErrorLogger.SendDebug($"DrawElements called with {_indices.Length} indices.", "Chunk.cs (Terrarium)", "NetworkListener");

            GL.BindVertexArray(0);
            ErrorLogger.SendDebug("Vertex Array Object unbound.", "Chunk.cs (Terrarium)", "NetworkListener");

            lock (_lock)
            {
                if(renderDebugCount == 0)
                {   
                    ErrorLogger.SendDebug("Chunk rendered successfully.", "Chunk.cs (Terrarium)", "NetworkListener");
                    renderDebugCount++;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Chunk rendering exception: {ex.Message}", "Chunk.cs (Terrarium)", "NetworkListener");
        }
    }

    

    public void RenderWireframe()
    {
        try
        {
            _shader?.Use();
            _shader?.SetMatrix4("model", Matrix4.Identity);
            _shader?.SetMatrix4("view", _camera.GetViewMatrix());
            _shader?.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.BindVertexArray(_vertexArrayObject);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.BindVertexArray(0);

            ErrorLogger.SendDebug("Chunk wireframe rendered.", "Chunk.cs (Terrarium)", "NetworkListener");
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Chunk wireframe rendering exception: {ex.Message}", "Chunk.cs (Terrarium)", "NetworkListener");
        }
    }
}