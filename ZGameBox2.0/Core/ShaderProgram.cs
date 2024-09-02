using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;
using Error;

public class Shader
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        try
        {
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            ErrorLogger.SendError("Loading shader sources from files.", "ShaderProgram.cs (Terrarium)", "NetworkListener");

            // Load the shader sources from files
            if (!File.Exists(vertexPath))
            {
                throw new FileNotFoundException("Vertex shader file not found.", vertexPath);
            }
            if (!File.Exists(fragmentPath))
            {
                throw new FileNotFoundException("Fragment shader file not found.", fragmentPath);
            }
            

            // Compile the vertex shader
            ErrorLogger.SendError("Compiling vertex shader.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompilation(vertexShader);

            // Compile the fragment shader
            ErrorLogger.SendError("Compiling fragment shader.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompilation(fragmentShader);

            // Link the shaders into a program
            ErrorLogger.SendError("Linking shaders into a program.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckProgramLinking(Handle);

            // Clean up the shaders as they're no longer needed after linking
            ErrorLogger.SendError("Cleaning up shaders.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            ErrorLogger.SendError("Shader program created successfully.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Exception: {ex.Message}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            throw;
        }
    }

    private void CheckShaderCompilation(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            ErrorLogger.SendError($"Shader Compilation Error: {infoLog}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            throw new Exception($"Shader Compilation Error: {infoLog}");
        }
        else
        {
            ErrorLogger.SendError("Shader compiled successfully.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
        }
    }

    private void CheckProgramLinking(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            ErrorLogger.SendError($"Program Linking Error: {infoLog}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            throw new Exception($"Program Linking Error: {infoLog}");
        }
        else
        {
            ErrorLogger.SendError("Program linked successfully.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
        }
    }

    public void Use()
    {
        try
        {
            GL.UseProgram(Handle);
            ErrorLogger.SendError("Shader program is now in use.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Exception when using shader program: {ex.Message}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            throw;
        }
    }
}
