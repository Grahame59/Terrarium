using OpenTK.Mathematics;

public class Camera
{
    public Matrix4 View { get; private set; }
    public Matrix4 Projection { get; private set; }

    public Camera(Vector3 position, Vector3 target, Vector3 up, float aspectRatio)
    {
        View = Matrix4.LookAt(position, target, up);
        Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 100f);
    }

    public void Update()
    {
        //update the view matrix based on camera movement here
    }
}
