using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Shot.Rendering.Objects;

public class ShotCamera
{
    public Matrix4 Projection;
    public Matrix4 View;
    
    private readonly Vector3 _worldUp = Vector3.UnitY;
    private Vector3 _cameraUp = Vector3.UnitY;
    private Vector3 _cameraForward = -Vector3.UnitZ;
    private Vector3 _cameraRight = Vector3.UnitX;
    
    public Vector3 Position { get; private set; } = new Vector3(0, 0, 6);
    public float Yaw = -90.0f;
    public float Pitch;
    public float Fov { get; private set; } = 45.0f;

    private bool _canRotate = true;
    private const float Speed = 6;
    public float AspectRatio;

    public void UpdateCamera()
    { 
        View = Matrix4.LookAt(Position, Position + _cameraForward, _cameraUp);
        Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov),
            AspectRatio,
            0.1f, 100f);
    }

    public void SetCamera()
    {
        Position = View.ExtractTranslation();
    }
        
    public void MoveCamera(Vector3 moveInput, float delta, float scrollDelta)
    {
        Position += _cameraRight * moveInput.X * Speed * delta;
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
        front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        _cameraForward = Vector3.Normalize(front);
        _cameraRight = Vector3.Normalize(Vector3.Cross(_cameraForward, _worldUp));
        _cameraUp = Vector3.Normalize(Vector3.Cross(_cameraRight, _cameraForward));
    }
    

    public CursorState LockCamera()
    {
            _canRotate = !_canRotate;
            return _canRotate ? CursorState.Grabbed : CursorState.Normal;
    }
}