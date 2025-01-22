using OpenTK.Mathematics;

namespace Shot.Rendering.Objects;

public struct ShotTransform
{
    public Vector3 Position;
    public Quaternion Rotation;

    public ShotTransform(Vector3 position)
    {
        Position = position;
        Rotation = Quaternion.Identity;
    }
}