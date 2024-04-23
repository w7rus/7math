using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace _7math;

public struct Quaternion : IEquatable<Quaternion>, IFormattable
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double W { get; set; }

    public Quaternion(double x = 0, double y = 0, double z = 0, double w = 0)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
    
    public static Quaternion UnitX => new(1);
    public static Quaternion UnitY => new(y: 1);
    public static Quaternion UnitZ => new(z: 1);
    public static Quaternion UnitW => new(w: 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Clone()
    {
        return new Quaternion(X, Y, Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Copy(Quaternion quat)
    {
        X = quat.X;
        Y = quat.Y;
        Z = quat.Z;
        W = quat.W;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Set(double x = 0, double y = 0, double z = 0, double w = 0)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetIdentity() => Copy(UnitW);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double AngleTo(Quaternion quat) => 2 * Math.Acos(Math.Abs(Helpers.Clamp(Dot(quat), -1, 1)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Conjugate()
    {
        X *= -1;
        Y *= -1;
        Z *= -1;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Dot(Quaternion quat) => X * quat.X + Y * quat.Y + Z * quat.Z + W * quat.W;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetFromArray(double[] arr, int offset = 0)
    {
        X = arr[offset];
        Y = arr[offset + 1];
        Z = arr[offset + 2];
        W = arr[offset + 3];

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Length() => Math.Sqrt(LengthSquared());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double LengthSquared() => Dot(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Normalize()
    {
        var length = Length();

        if (length == 0)
            Copy(UnitW);
        else
        {
            length = 1 / length;
            X *= length;
            Y *= length;
            Z *= length;
            W *= length;
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Multiply(Quaternion quat)
    {
        var nX1 = X;
        var nY1 = Y;
        var nZ1 = Z;
        var nW1 = W;

        var nX2 = quat.X;
        var nY2 = quat.Y;
        var nZ2 = quat.Z;
        var nW2 = quat.W;

        X = nX1 * nW2 + nW1 * nX2 + nY1 * nZ2 - nZ1 * nY2;
        Y = nY1 * nW2 + nW1 * nY2 + nZ1 * nX2 - nX1 * nZ2;
        Z = nZ1 * nW2 + nW1 * nZ2 + nX1 * nY2 - nY1 * nX2;
        W = nW1 * nW2 - nX1 * nX2 - nY1 * nY2 - nZ1 * nZ2;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Premultiply(Quaternion quat)
    {
        var nX1 = quat.X;
        var nY1 = quat.Y;
        var nZ1 = quat.Z;
        var nW1 = quat.W;

        var nX2 = X;
        var nY2 = Y;
        var nZ2 = Z;
        var nW2 = W;

        X = nX1 * nW2 + nW1 * nX2 + nY1 * nZ2 - nZ1 * nY2;
        Y = nY1 * nW2 + nW1 * nY2 + nZ1 * nX2 - nX1 * nZ2;
        Z = nZ1 * nW2 + nW1 * nZ2 + nX1 * nY2 - nY1 * nX2;
        W = nW1 * nW2 - nX1 * nX2 - nY1 * nY2 - nZ1 * nZ2;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion RotateTowards(Quaternion quat, double theta)
    {
        var angle = AngleTo(quat);

        return angle == 0 ? this : Slerp(quat, Math.Min(1, theta / angle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Slerp(Quaternion quat, double alpha)
    {
        var x = X;
        var y = Y;
        var z = Z;
        var w = W;

        var cosHalfTheta = Dot(quat);

        if (cosHalfTheta < 0)
        {
            X = -quat.X;
            Y = -quat.Y;
            Z = -quat.Z;
            W = -quat.W;
            cosHalfTheta = -cosHalfTheta;
        }
        else
            Copy(quat);

        var sqrSinHalfTheta = 1 - cosHalfTheta * cosHalfTheta;

        if (sqrSinHalfTheta <= double.Epsilon)
        {
            var invAlpha = 1 - alpha;
            X = invAlpha * x + alpha * X;
            Y = invAlpha * y + alpha * Y;
            Z = invAlpha * z + alpha * Z;
            W = invAlpha * w + alpha * W;

            return Normalize();
        }

        var sinHalfTheta = Math.Sqrt(sqrSinHalfTheta);
        var halfTheta = Math.Atan2(sinHalfTheta, cosHalfTheta);
        var a = Math.Sin((1 - alpha) * halfTheta) / sinHalfTheta;
        var b = Math.Sin(alpha * halfTheta) / sinHalfTheta;

        X = x * a + X * b;
        Y = y * a + Y * b;
        Z = z * a + Z * b;
        W = w * a + W * b;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetFromAxisAngle(Vector3 vec3, double theta)
    {
        var halfTheta = theta / 2;
        var sinHalfTheta = Math.Sin(halfTheta);
        X = vec3.X * sinHalfTheta;
        Y = vec3.Y * sinHalfTheta;
        Z = vec3.Z * sinHalfTheta;
        W = Math.Cos(halfTheta);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetFromEuler(Euler eul)
    {
        var cosX = Math.Cos(eul.X / 2);
        var cosY = Math.Cos(eul.Y / 2);
        var cosZ = Math.Cos(eul.Z / 2);
        var sinX = Math.Sin(eul.X / 2);
        var sinY = Math.Sin(eul.Y / 2);
        var sinZ = Math.Sin(eul.Z / 2);

        switch (eul.Order)
        {
            case AxisOrder.XYZ:
                X = sinX * cosY * cosZ + cosX * sinY * sinZ;
                Y = cosX * sinY * cosZ - sinX * cosY * sinZ;
                Z = cosX * cosY * sinZ + sinX * sinY * cosZ;
                W = cosX * cosY * cosZ - sinX * sinY * sinZ;
                break;
            case AxisOrder.YXZ:
                X = sinX * cosY * cosZ + cosX * sinY * sinZ;
                Y = cosX * sinY * cosZ - sinX * cosY * sinZ;
                Z = cosX * cosY * sinZ - sinX * sinY * cosZ;
                W = cosX * cosY * cosZ + sinX * sinY * sinZ;
                break;
            case AxisOrder.ZXY:
                X = sinX * cosY * cosZ - cosX * sinY * sinZ;
                Y = cosX * sinY * cosZ + sinX * cosY * sinZ;
                Z = cosX * cosY * sinZ + sinX * sinY * cosZ;
                W = cosX * cosY * cosZ - sinX * sinY * sinZ;
                break;
            case AxisOrder.ZYX:
                X = sinX * cosY * cosZ - cosX * sinY * sinZ;
                Y = cosX * sinY * cosZ + sinX * cosY * sinZ;
                Z = cosX * cosY * sinZ - sinX * sinY * cosZ;
                W = cosX * cosY * cosZ + sinX * sinY * sinZ;
                break;
            case AxisOrder.YZX:
                X = sinX * cosY * cosZ + cosX * sinY * sinZ;
                Y = cosX * sinY * cosZ + sinX * cosY * sinZ;
                Z = cosX * cosY * sinZ - sinX * sinY * cosZ;
                W = cosX * cosY * cosZ - sinX * sinY * sinZ;
                break;
            case AxisOrder.XZY:
                X = sinX * cosY * cosZ - cosX * sinY * sinZ;
                Y = cosX * sinY * cosZ - sinX * cosY * sinZ;
                Z = cosX * cosY * sinZ + sinX * sinY * cosZ;
                W = cosX * cosY * cosZ + sinX * sinY * sinZ;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetFromRotationMatrix(Matrix4 mat4)
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

        var trace = n11 + n22 + n33;

        if (trace > 0)
        {
            var multiplier = 0.5f / Math.Sqrt(trace + 1);
            X = (n32 - n23) * multiplier;
            Y = (n13 - n32) * multiplier;
            Z = (n21 - n12) * multiplier;
            W = 0.25f / multiplier;
        }
        else if (n11 > n22 && n11 > n33)
        {
            var multiplier = 2 * Math.Sqrt(1 + n11 - n22 - n33);
            X = 0.25f * multiplier;
            Y = (n12 + n21) / multiplier;
            Z = (n13 + n32) / multiplier;
            W = (n32 - n23) / multiplier;
        }
        else if (n22 > n33)
        {
            var multiplier = 2 * Math.Sqrt(1 + n22 - n11 - n33);
            X = (n12 + n21) / multiplier;
            Y = 0.25f * multiplier;
            Z = (n23 + n32) / multiplier;
            W = (n13 - n32) / multiplier;
        }
        else
        {
            var multiplier = 2 * Math.Sqrt(1 + n33 - n11 - n22);
            X = (n13 + n31) / multiplier;
            Y = (n23 + n32) / multiplier;
            Z = 0.25f * multiplier;
            W = (n21 - n12) / multiplier;
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetFromVectors(Vector3 vec3From, Vector3 vec3To)
    {
        var r = vec3From.Dot(vec3To) + 1;

        if (r < double.Epsilon)
        {
            if (Math.Abs(vec3From.X) > Math.Abs(vec3From.Z))
            {
                X = -vec3From.Y;
                Y = vec3From.X;
                Z = 0;
                W = 0;
            }
            else
            {
                X = 0;
                Y = -vec3From.Z;
                Z = vec3From.Y;
                W = 0;
            }
        }
        else
        {
            X = vec3From.Y * vec3To.Z - vec3From.Z * vec3To.Y;
            Y = vec3From.Z * vec3To.X - vec3From.X * vec3To.Z;
            Z = vec3From.X * vec3To.Y - vec3From.Y * vec3To.X;
            W = r;
        }

        return Normalize();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double[] ToArray() => new[] { X, Y, Z, W };

    public bool Equals(Quaternion other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    public override bool Equals(object obj) => obj is Quaternion other && Equals(other);

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);

    public readonly string ToString(string format, IFormatProvider formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return
            $"quat<{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}{separator} {Z.ToString(format, formatProvider)}{separator} {W.ToString(format, formatProvider)}>";
    }

    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);
}