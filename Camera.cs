using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Camera
{
    private Vector3 _position;
    private Vector3 _target;
    private Vector3 _up;
    private float _aspectRatio;
    private float _radius = 10.0f; // Distance from the target
    private float _angle = 0.0f; // Rotation angle

    public Matrix4 View { get; private set; }
    public Matrix4 Projection { get; private set; }

    public Camera(Vector3 position, Vector3 target, Vector3 up, float aspectRatio)
    {
        _position = position;
        _target = target;
        _up = up;
        _aspectRatio = aspectRatio;
        Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 100f);
        UpdateViewMatrix();
    }

    public void UpdateKeyboardInput(KeyboardState keyboardState)
    {
        float angleSpeed = 0.02f; // Speed of rotation

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            _angle -= angleSpeed;
        }
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            _angle += angleSpeed;
        }

        // Update camera position based on angle
        _position.X = _target.X + _radius * (float)Math.Cos(_angle);
        _position.Z = _target.Z + _radius * (float)Math.Sin(_angle);
        //_position.Y = _target.Y + _radius * 0.5f; // Optional: Adjust vertical position

        UpdateViewMatrix();
    }

    private void UpdateViewMatrix()
    {
        View = Matrix4.LookAt(_position, _target, _up);
    }
}
