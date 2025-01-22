using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Shot.Rendering.Objects;

public class ShotCamera
{
    public Matrix4 Projection => Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov),
        800.0f / 600.0f,
        0.1f, 100f);
    private readonly Vector3 _cameraUp = Vector3.UnitY;
    public Vector3 Position { get; private set; } = new Vector3(0, 0, 6);
    public Matrix4 View => Matrix4.LookAt(Position, Position + _cameraForward, _cameraUp);
    private Vector3 _cameraForward = -Vector3.UnitZ;
    public float Yaw = -90.0f;
    public float Pitch;
    public float Fov { get; private set; } = 45.0f;

    private bool _canRotate = true;
    private const float Speed = 6;

    public void MoveCamera(Vector3 moveInput, float delta, float scrollDelta)
    {
        Position += Vector3.Normalize(Vector3.Cross(_cameraForward, _cameraUp)) * moveInput.X * Speed * delta;
        Position += _cameraForward * moveInput.Y * Speed * delta;
        Position += _cameraUp * moveInput.Z * Speed * delta;
        Fov = Math.Clamp(Fov - scrollDelta, 1, 45);
    }

    public void RotateCamera(float moveX, float moveY)
    {
        if (!_canRotate)
        {
            return;
        }

        Vector3 front;
        
        Yaw += moveX;
        Pitch -= moveY;

        Pitch = Math.Clamp(Pitch, -89f, 89f);
        front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw )) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw )) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        _cameraForward = Vector3.Normalize(front);
    }
    

    public CursorState LockCamera()
    {
            _canRotate = !_canRotate;
            return _canRotate ? CursorState.Grabbed : CursorState.Normal;
    }
}