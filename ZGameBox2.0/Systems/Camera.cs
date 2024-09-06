using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Error;

public class Camera
{
    private Vector3 _position;
    private Vector3 _target;
    private Vector3 _up;
    private float _aspectRatio;
    private float _radius = 10.0f;
    private float _angle = 0.0f;

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateViewMatrix();
        }
    }

    public Matrix4 View { get; private set; }
    public Matrix4 Projection { get; private set; }

    public Camera(Vector3 position, Vector3 target, Vector3 up, float aspectRatio)
    {
        _position = position;
        _target = target;
        _up = up;
        _aspectRatio = aspectRatio;
        Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 100f);

        ErrorLogger.SendDebug("Camera initialized", "Camera.cs (Terrarium)", "NetworkListener");

        UpdateViewMatrix();
    }

    public void UpdateKeyboardInput(KeyboardState keyboardState)
    {
        float angleSpeed = 0.02f;

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            _angle -= angleSpeed;
        }
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            _angle += angleSpeed;
        }

        _position.X = _target.X + _radius * (float)Math.Cos(_angle);
        _position.Z = _target.Z + _radius * (float)Math.Sin(_angle);

        UpdateViewMatrix();
    }

    private void UpdateViewMatrix()
    {
        View = Matrix4.LookAt(_position, _target, _up);
    }

    // Add these methods to match the expected interface
    public Matrix4 GetViewMatrix()
    {
        return View;
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Projection;
    }
}