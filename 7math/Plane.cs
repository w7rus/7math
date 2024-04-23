using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace _7math;

public struct Plane : IEquatable<Plane>, IFormattable
{
    public Vector3 Normal { get; set; }
    public double Constant { get; set; }

    public Plane(Vector3? normal = null, double constant = 0)
    {
        Normal = normal ?? Vector3.UnitX;
        Constant = constant;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane Clone() => new(Normal, Constant);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane Copy(Plane plane)
    {
        Normal = plane.Normal;
        Constant = plane.Constant;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane Set(Vector3? normal = null, double constant = 0)
    {
        Normal = normal ?? Vector3.UnitX;
        Constant = constant;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane SetFromNormalAndCoplanarPoint(Vector3 vec3Normal, Vector3 vec3)
    {
        Normal.Copy(vec3Normal);
        Constant = -vec3.Dot(Normal);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane SetFromCoplanarPoints(Vector3 vec31, Vector3 vec32, Vector3 vec33) =>
        SetFromNormalAndCoplanarPoint(vec33
            .Subtract(vec32)
            .Cross(vec31.Clone().Subtract(vec32))
            .Normalize(), vec31);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane Normalize()
    {
        var normalLengthInverse = 1 / Normal.Length();
        Normal.MultiplyScalar(normalLengthInverse);
        Constant *= normalLengthInverse;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane Negate()
    {
        Constant *= -1;
        Normal.Negate();

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double DistanceToPoint(Vector3 vec3) =>
        Normal.Dot(vec3) + Constant;

    public double DistanceToSphere(Sphere sphere)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetProjectedPoint(Vector3 vec3) =>
        Normal.Clone().MultiplyScalar(-DistanceToPoint(vec3)).Add(vec3);

    public Vector3 GetLineIntersection(Line line)
    {
        throw new NotImplementedException();
    }

    public bool IsIntersectingLine(Line line)
    {
        throw new NotImplementedException();
    }

    public bool IsIntersectingBox(Box box)
    {
        throw new NotImplementedException();
    }

    public bool IsIntersectingSphere(Sphere sphere)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetCoplanarPoint() => Normal.Clone().MultiplyScalar(-Constant);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane ApplyMatrix4(Matrix4 mat4, Matrix3? mat3Normal)
    {
        mat3Normal ??= new Matrix3().GetNormalMatrix(mat4);
        var vec3Reference = GetCoplanarPoint().ApplyMatrix4(mat4);
        var vec3Normal = Normal.ApplyMatrix3(mat3Normal.Value).Normalize();
        Constant = -vec3Reference.Dot(vec3Normal);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane Translate(Vector3 vec3)
    {
        Constant -= vec3.Dot(Normal);

        return this;
    }

    public bool Equals(Plane other)
    {
        return Normal.Equals(other.Normal) && Constant.Equals(other.Constant);
    }

    public override bool Equals(object obj)
    {
        return obj is Plane other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Normal.GetHashCode(), Constant);
    }

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);

    public readonly string ToString(string format, IFormatProvider formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return
            $"plane<{Normal.ToString(format, formatProvider)}{separator} {Constant.ToString(format, formatProvider)}>";
    }
}