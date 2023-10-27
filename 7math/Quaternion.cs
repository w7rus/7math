using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace _7math;

public struct Quaternion : IEquatable<Quaternion>, IFormattable
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }

    public Quaternion(float x = 0, float y = 0, float z = 0, float w = 0)
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
    public Quaternion Set(float x = 0, float y = 0, float z = 0, float w = 0)
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
    public float AngleTo(Quaternion quat) => 2 * MathF.Acos(MathF.Abs(Helpers.Clamp(Dot(quat), -1, 1)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Conjugate()
    {
        X *= -1;
        Y *= -1;
        Z *= -1;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Dot(Quaternion quat) => X * quat.X + Y * quat.Y + Z * quat.Z + W * quat.W;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetFromArray(float[] arr, int offset = 0)
    {
        X = arr[offset];
        Y = arr[offset + 1];
        Z = arr[offset + 2];
        W = arr[offset + 3];

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Length() => MathF.Sqrt(LengthSquared());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float LengthSquared() => Dot(this);

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
    public Quaternion RotateTowards(Quaternion quat, float theta)
    {
        var angle = AngleTo(quat);

        return angle == 0 ? this : Slerp(quat, MathF.Min(1, theta / angle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Slerp(Quaternion quat, float alpha)
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

        if (sqrSinHalfTheta <= float.Epsilon)
        {
            var invAlpha = 1 - alpha;
            X = invAlpha * x + alpha * X;
            Y = invAlpha * y + alpha * Y;
            Z = invAlpha * z + alpha * Z;
            W = invAlpha * w + alpha * W;

            return Normalize();
        }

        var sinHalfTheta = MathF.Sqrt(sqrSinHalfTheta);
        var halfTheta = MathF.Atan2(sinHalfTheta, cosHalfTheta);
        var a = MathF.Sin((1 - alpha) * halfTheta) / sinHalfTheta;
        var b = MathF.Sin(alpha * halfTheta) / sinHalfTheta;

        X = x * a + X * b;
        Y = y * a + Y * b;
        Z = z * a + Z * b;
        W = w * a + W * b;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetFromAxisAngle(Vector3 vec3, float theta)
    {
        var halfTheta = theta / 2;
        var sinHalfTheta = MathF.Sin(halfTheta);
        X = vec3.X * sinHalfTheta;
        Y = vec3.Y * sinHalfTheta;
        Z = vec3.Z * sinHalfTheta;
        W = MathF.Cos(halfTheta);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion SetFromEuler(Euler eul)
    {
        var cosX = MathF.Cos(eul.X / 2);
        var cosY = MathF.Cos(eul.Y / 2);
        var cosZ = MathF.Cos(eul.Z / 2);
        var sinX = MathF.Sin(eul.X / 2);
        var sinY = MathF.Sin(eul.Y / 2);
        var sinZ = MathF.Sin(eul.Z / 2);

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
            var multiplier = 0.5f / MathF.Sqrt(trace + 1);
            X = (n32 - n23) * multiplier;
            Y = (n13 - n32) * multiplier;
            Z = (n21 - n12) * multiplier;
            W = 0.25f / multiplier;
        }
        else if (n11 > n22 && n11 > n33)
        {
            var multiplier = 2 * MathF.Sqrt(1 + n11 - n22 - n33);
            X = 0.25f * multiplier;
            Y = (n12 + n21) / multiplier;
            Z = (n13 + n32) / multiplier;
            W = (n32 - n23) / multiplier;
        }
        else if (n22 > n33)
        {
            var multiplier = 2 * MathF.Sqrt(1 + n22 - n11 - n33);
            X = (n12 + n21) / multiplier;
            Y = 0.25f * multiplier;
            Z = (n23 + n32) / multiplier;
            W = (n13 - n32) / multiplier;
        }
        else
        {
            var multiplier = 2 * MathF.Sqrt(1 + n33 - n11 - n22);
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

        if (r < float.Epsilon)
        {
            if (MathF.Abs(vec3From.X) > MathF.Abs(vec3From.Z))
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
    public float[] ToArray() => new[] { X, Y, Z, W };

    public bool Equals(Quaternion other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    public override bool Equals(object obj) => obj is Quaternion other && Equals(other);

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format) => ToString(format, CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format, IFormatProvider formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return
            $"quat<{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}{separator} {Z.ToString(format, formatProvider)}{separator} {W.ToString(format, formatProvider)}>";
    }

    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);
}