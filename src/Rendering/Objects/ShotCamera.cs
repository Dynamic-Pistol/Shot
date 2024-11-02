using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Shot.Rendering.Objects;

public class ShotCamera
{
    public Matrix4 Projection;
    private readonly Vector3 _cameraUp = Vector3.UnitY;
    public Vector3 Position { get; private set; } = Vector3.Zero;
    public Matrix4 View;
    private Vector3 _cameraForward = -Vector3.UnitZ;
    private float _fov = MathHelper.PiOver2;
    public float Yaw;
    public float Pitch;
    private bool _canRotate = true;
    private const float Speed = 6;

    public void MoveCamera(Vector2 moveInput, float delta, float scrollDelta)
    {
        Position += Vector3.Normalize(Vector3.Cross(_cameraForward, _cameraUp)) * moveInput.X * Speed * delta;
        Position += _cameraForward * moveInput.Y * Speed * delta;
        _fov = Math.Clamp(_fov - scrollDelta, 1, 45);
        Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov),
            800.0f / 600.0f,
            0.1f, 100f);
        View = Matrix4.LookAt(Position, Position + _cameraForward, _cameraUp);
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