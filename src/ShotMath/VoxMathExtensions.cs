using OpenTK.Mathematics;

namespace Shot.ShotMath;

public static class VoxMathExtensions
{
    
    public static System.Numerics.Vector3 OpenTk2Sys(this Vector3 original)
    {
        return new System.Numerics.Vector3(original.X, original.Y, original.Z);
    }

    public static Vector3 Sys2OpenTk(this System.Numerics.Vector3 original)
    {
        return new Vector3(original.X, original.Y, original.Z);
    }
}
