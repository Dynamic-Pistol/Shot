using OpenTK.Mathematics;

namespace Shot.Rendering.Objects;

public struct ShotTransform(Vector3 position)
{
    public Vector3 Position = position;
    public Vector3 EularRotation = Vector3.Zero;
    public Vector3 Scale = Vector3.One;
}