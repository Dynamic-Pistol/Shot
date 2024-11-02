using OpenTK.Mathematics;

namespace Shot.Rendering.Objects;


public struct ShotVertex(float x, float y, float z)
{
    public Vector3 Position = new Vector3(x, y, z);

    public static implicit operator ShotVertex((float x, float y, float z) srcTuple)
    {
        return new ShotVertex(srcTuple.x, srcTuple.y, srcTuple.z);
    }
}