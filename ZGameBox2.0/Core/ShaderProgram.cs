using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;
using OpenTK.Mathematics;
using Error;

public class Shader
{
    public int Handle;
    public int shaderDebugCount = 0;

    public Shader(string vertexPath, string fragmentPath)
    {
        try
        {
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            ErrorLogger.SendDebug("Loading shader sources from files.", "ShaderProgram.cs (Terrarium)", "NetworkListener");

            // Load the shader sources from files
            if (!File.Exists(vertexPath))
            {
                ErrorLogger.SendError($"Vertex shader file not found. {vertexPath}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
                throw new FileNotFoundException("Vertex shader file not found.", vertexPath);
            }
            if (!File.Exists(fragmentPath))
            {
                ErrorLogger.SendError($"Vertex shader file not found. {fragmentPath}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
                throw new FileNotFoundException("Fragment shader file not found.", fragmentPath);
            }
            
            // Compile the vertex shader
            ErrorLogger.SendDebug("Compiling vertex shader.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompilation(vertexShader);

            // Compile the fragment shader
            ErrorLogger.SendDebug("Compiling fragment shader.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompilation(fragmentShader);

            // Link the shaders into a program
            ErrorLogger.SendDebug("Linking shaders into a program.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckProgramLinking(Handle);

            // Clean up the shaders as they're no longer needed after linking
            ErrorLogger.SendDebug("Cleaning up shaders.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            ErrorLogger.SendDebug("Shader program created successfully.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
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
            ErrorLogger.SendDebug("Shader compiled successfully.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
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
            ErrorLogger.SendDebug("Program linked successfully.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
        }
    }

    public void Use()
    {
        try
        {
            GL.UseProgram(Handle);
            if (shaderDebugCount == 0)
            {
                ErrorLogger.SendDebug("Shader program is now in use.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
                shaderDebugCount++;
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Exception when using shader program: {ex.Message}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            throw;
        }
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        try
        {
            int location = GL.GetUniformLocation(Handle, name);

            if (location == -1)
            {
                ErrorLogger.SendError($"Uniform '{name}' not found in shader program.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
                return;
            }

            GL.UniformMatrix4(location, false, ref matrix);

            if (shaderDebugCount == 0)
            {
                ErrorLogger.SendDebug($"Matrix uniform '{name}' set successfully.", "ShaderProgram.cs (Terrarium)", "NetworkListener");
                shaderDebugCount++;
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Exception when setting matrix uniform '{name}': {ex.Message}", "ShaderProgram.cs (Terrarium)", "NetworkListener");
            throw;
        }
    }
    public void SetVector3(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(location, value.X, value.Y, value.Z);
    }
}
