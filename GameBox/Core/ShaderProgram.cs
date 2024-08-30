using Error;
using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

public class ShaderProgram
{
    private readonly int _handle;

    public ShaderProgram(string vertexPath, string fragmentPath)
    {
        var vertexShader = LoadShader(vertexPath, ShaderType.VertexShader);
        var fragmentShader = LoadShader(fragmentPath, ShaderType.FragmentShader);

        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int linkStatus);
        if (linkStatus == 0)
        {
            var log = GL.GetProgramInfoLog(_handle);
            ErrorLogger.SendError($"Program link error: {log}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            throw new Exception($"Program link error: {log}");
        }

        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public int Handle => _handle; // Expose _handle via a public property

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

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int compileStatus);
        if (compileStatus == 0)
        {
            var log = GL.GetShaderInfoLog(shader);
            ErrorLogger.SendError($"Shader compiliation error ({type}): {log}", "ShaderProgram.cs", "NetworkListener");
            throw new Exception($"Shader compilation error ({type}): {log}");
        }

        return shader;
    }
}
