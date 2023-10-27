namespace _7math;

public enum AxisOrder
{
    XYZ,
    YXZ,
    ZXY,

    /// <summary>
    /// Most used in 3D graphics where X is Right, Z is Up and Y is pointed away from viewer (ex. Half-Life 2)
    /// </summary>
    /// <remarks>First Yaw around Z, then Pitch around Y, then Roll around Z</remarks>
    ZYX,

    /// <summary>
    /// Most used in 3D graphics where X is Right, Y is Up and Z is pointed to viewer (ex. Minecraft)
    /// </summary>
    /// <remarks>First Yaw around Y, then Pitch around Z, then Roll around X</remarks>
    YZX,
    XZY
}