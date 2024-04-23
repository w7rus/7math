using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace _7math;

public struct Euler : IEquatable<Euler>, IFormattable
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public AxisOrder Order { get; set; }

    private static readonly double _oneMinusEpsilon = 1 - double.Epsilon;

    public Euler(double x = 0, double y = 0, double z = 0, AxisOrder order = AxisOrder.ZYX)
    {
        X = x;
        Y = y;
        Z = z;
        Order = order;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Euler Clone() => new(X, Y, Z, Order);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Euler Copy(Euler eul)
    {
        X = eul.X;
        Y = eul.Y;
        Z = eul.Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Euler Set(double x = 0, double y = 0, double z = 0, AxisOrder order = AxisOrder.ZYX)
    {
        X = x;
        Y = y;
        Z = z;
        Order = order;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Euler Reorder(AxisOrder order) => SetFromQuaternion(new Quaternion().SetFromEuler(this), order);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Euler SetFromRotationMatrix(Matrix4 mat4, AxisOrder? order = null)
    {
        var n11 = mat4.Elements[0];
        var n12 = mat4.Elements[4];
        var n13 = mat4.Elements[8];
        var n21 = mat4.Elements[1];
        var n22 = mat4.Elements[5];
        var n23 = mat4.Elements[9];
        var n31 = mat4.Elements[2];
        var n32 = mat4.Elements[6];
        var n33 = mat4.Elements[10];

        var axisOrder = order ?? Order;

        switch (axisOrder)
        {
            case AxisOrder.XYZ:
                Y = Math.Asin(Helpers.Clamp(n13, -1, 1));
                if (Math.Abs(n13) < _oneMinusEpsilon)
                {
                    X = Math.Atan2(-n23, n33);
                    Z = Math.Atan2(-n12, n11);
                }
                else
                {
                    X = Math.Atan2(n32, n22);
                    Z = 0;
                }

                break;
            case AxisOrder.YXZ:
                X = Math.Asin(-Helpers.Clamp(n23, -1, 1));
                if (Math.Abs(n23) < _oneMinusEpsilon)
                {
                    Y = Math.Atan2(n13, n33);
                    Z = Math.Atan2(n21, n22);
                }
                else
                {
                    Y = Math.Atan2(-n31, n11);
                    Z = 0;
                }

                break;
            case AxisOrder.ZXY:
                X = Math.Asin(Helpers.Clamp(n32, -1, 1));
                if (Math.Abs(n32) < _oneMinusEpsilon)
                {
                    Y = Math.Atan2(-n31, n33);
                    Z = Math.Atan2(-n12, n22);
                }
                else
                {
                    Y = 0;
                    Z = Math.Atan2(n21, n11);
                }

                break;
            case AxisOrder.ZYX:
                Y = Math.Asin(-Helpers.Clamp(n31, -1, 1));
                if (Math.Abs(n31) < _oneMinusEpsilon)
                {
                    X = Math.Atan2(n32, n33);
                    Z = Math.Atan2(n21, n22);
                }
                else
                {
                    X = 0;
                    Z = Math.Atan2(-n12, n22);
                }

                break;
            case AxisOrder.YZX:
                Z = Math.Asin(Helpers.Clamp(n21, -1, 1));
                if (Math.Abs(n21) < _oneMinusEpsilon)
                {
                    X = Math.Atan2(-n23, n22);
                    Y = Math.Atan2(-n31, n11);
                }
                else
                {
                    X = 0;
                    Y = Math.Atan2(n13, n33);
                }

                break;
            case AxisOrder.XZY:
                Z = Math.Asin(-Helpers.Clamp(n12, -1, 1));
                if (Math.Abs(n12) < _oneMinusEpsilon)
                {
                    X = Math.Atan2(n32, n22);
                    Y = Math.Atan2(n13, n11);
                }
                else
                {
                    X = Math.Atan2(-n23, n33);
                    Y = 0;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Order = axisOrder;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Euler SetFromQuaternion(Quaternion quat, AxisOrder? order = null) => SetFromRotationMatrix(new Matrix4().MakeRotationFromQuaternion(quat), order);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Euler SetFromVectorAndOrder(Vector3 vec3, AxisOrder order = AxisOrder.ZYX) => Set(vec3.X, vec3.Y, vec3.Z, order);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Euler other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && Order == other.Order;

    public override bool Equals(object obj) => obj is Euler other && Equals(other);

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);

    public readonly string ToString(string format, IFormatProvider formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return
            $"eul<{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}{separator} {Z.ToString(format, formatProvider)}{separator} {Order.ToString()}>";
    }

    public override int GetHashCode() => HashCode.Combine(X, Y, Z, Order);
}