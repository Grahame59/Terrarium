using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

public class ShaderProgram
{
    private readonly int _handle;

    public ShaderProgram(string vertexPath, string fragmentPath)
    {
        // Load and compile shaders
        var vertexShader = LoadShader(vertexPath, ShaderType.VertexShader);
        var fragmentShader = LoadShader(fragmentPath, ShaderType.FragmentShader);

        // Create and link the shader program
        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        GL.LinkProgram(_handle);

        // Check for linking errors
        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int linkStatus);
        if (linkStatus == 0)
        {
            var log = GL.GetProgramInfoLog(_handle);
            throw new Exception($"Program link error: {log}");
        }

        // Detach and delete shaders
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    private int LoadShader(string path, ShaderType type)
    {
        var shader = GL.CreateShader(type);

        var source = File.ReadAllText(path);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        // Check for compilation errors
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int compileStatus);
        if (compileStatus == 0)
        {
            var log = GL.GetShaderInfoLog(shader);
            throw new Exception($"Shader compilation error ({type}): {log}");
        }

        return shader;
    }
}
