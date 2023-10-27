using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace _7math;

public struct Triangle : IEquatable<Triangle>, IFormattable
{
    public Vector3 v1 { get; set; }
    public Vector3 v2 { get; set; }
    public Vector3 v3 { get; set; }

    public Triangle(Vector3? vec31 = null, Vector3? vec32 = null, Vector3? vec33 = null)
    {
        v1 = vec31 ?? Vector3.Zero;
        v2 = vec32 ?? Vector3.Zero;
        v3 = vec33 ?? Vector3.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Triangle Clone() => new(v1, v2, v3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Triangle Copy(Triangle triangle)
    {
        v1 = triangle.v1;
        v2 = triangle.v2;
        v3 = triangle.v3;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Triangle Set(Vector3? vec31 = null, Vector3? vec32 = null, Vector3? vec33 = null)
    {
        v1 = vec31 ?? Vector3.Zero;
        v2 = vec32 ?? Vector3.Zero;
        v3 = vec33 ?? Vector3.Zero;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetClosestToPoint(Vector3 vec3)
    {
        var vec3Cache1 = new Vector3();
        var vec3Cache2 = new Vector3();
        var vec3Cache3 = new Vector3();

        float t1;
        float t2;

        var vec3ab = v2.Clone().Subtract(v1);
        var vec3ac = v3.Clone().Subtract(v1);
        var vec3ap = vec3.Clone().Subtract(v1);
        var dotProduct1 = vec3ab.Dot(vec3ap);
        var dotProduct2 = vec3ac.Dot(vec3ap);

        if (dotProduct1 <= 0 && dotProduct2 <= 0)
            return v1;

        var vec3bp = vec3Cache3.Copy(vec3).Subtract(v2);
        var dotProduct3 = vec3ab.Dot(vec3bp);
        var dotProduct4 = vec3ac.Dot(vec3bp);
        if (dotProduct3 >= 0 && dotProduct4 <= dotProduct3)
            return v2;

        var vc = dotProduct1 * dotProduct4 - dotProduct3 * dotProduct2;
        if (vc <= 0 && dotProduct1 >= 0 && dotProduct3 <= 0)
        {
            t1 = dotProduct1 / (dotProduct1 - dotProduct3);
            return vec3Cache1.Copy(v1).Add(vec3Cache2.Copy(vec3ab).MultiplyScalar(t1));
        }

        var vec3cp = vec3Cache3.Copy(vec3).Subtract(v3);
        var dotProduct5 = vec3ab.Dot(vec3cp);
        var dotProduct6 = vec3ac.Dot(vec3cp);
        if (dotProduct6 >= 0 && dotProduct5 <= dotProduct6)
            return v3;

        var vb = dotProduct5 * dotProduct2 - dotProduct1 * dotProduct6;
        if (vb <= 0 && dotProduct2 >= 0 && dotProduct6 <= 0)
        {
            t2 = dotProduct2 / (dotProduct2 - dotProduct6);
            return vec3Cache1.Copy(v1).Add(vec3Cache2.Copy(vec3ac).MultiplyScalar(t2));
        }

        var va = dotProduct3 * dotProduct6 - dotProduct5 * dotProduct4;
        if (va <= 0 && (dotProduct4 - dotProduct3) >= 0 && (dotProduct5 - dotProduct6) >= 0)
        {
            var vec3bc = vec3Cache3.Copy(v3).Subtract(v2);
            t2 = (dotProduct4 - dotProduct3) / ((dotProduct4 - dotProduct3) + (dotProduct5 - dotProduct6));
            return vec3Cache1.Copy(v2).Add(vec3Cache2.Copy(vec3bc).MultiplyScalar(t2));
        }

        var denominator = 1 / (va + vb + vc);
        t1 = vb * denominator;
        t2 = vc * denominator;

        return new Vector3()
            .Copy(v1)
            .Add(vec3Cache1.Copy(vec3ab).MultiplyScalar(t1))
            .Add(vec3Cache2.Copy(vec3ac).MultiplyScalar(t2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsContainingPoint(Vector3 vec3)
    {
        var vec3Barycoord = GetBarycoord(vec3);

        if (!vec3Barycoord.HasValue)
            return false;

        return vec3Barycoord.Value is { X: >= 0, Y: >= 0 } && vec3Barycoord.Value.X + vec3Barycoord.Value.Z <= 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Area() =>
        new Vector3().Copy(v3).Subtract(v2).Cross(new Vector3().Copy(v1).Subtract(v2)).Length() * .5f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3? GetBarycoord(Vector3 vec3)
    {
        var vec31 = v3.Clone().Subtract(v1);
        var vec32 = v2.Clone().Subtract(v1);
        var vec33 = vec3.Subtract(v1);

        var dotProduct1 = vec31.Dot(vec31);
        var dotProduct2 = vec31.Dot(vec32);
        var dotProduct3 = vec31.Dot(vec33);
        var dotProduct4 = vec32.Dot(vec32);
        var dotProduct5 = vec32.Dot(vec33);

        var denominator = dotProduct1 * dotProduct4 - dotProduct2 * dotProduct2;

        if (denominator == 0)
            return null;

        var denominatorInverse = 1 / denominator;
        var u = (dotProduct4 * dotProduct3 - dotProduct2 * dotProduct5) * denominatorInverse;
        var v = (dotProduct1 * dotProduct5 - dotProduct2 * dotProduct3) * denominatorInverse;

        return new Vector3(1 - u - v, v, u);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetMidpoint() =>
        new Vector3().Copy(v1).Add(v2).Add(v3).MultiplyScalar(1f / 3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetNormal()
    {
        var vec3 = v3.Clone().Subtract(v2).Cross(v1.Clone().Subtract(v2));

        var lengthSquared = vec3.LengthSquared();
        return lengthSquared > 0 ? vec3.MultiplyScalar(1 / MathF.Sqrt(lengthSquared)) : Vector3.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane GetPlane() =>
        new Plane().SetFromCoplanarPoints(v1, v2, v3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsIntersectingBox(Box box)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFrontFacing(Vector3 vec3Direction) =>
        v3.Clone().Subtract(v2).Cross(v1.Clone().Subtract(v2)).Dot(vec3Direction) < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3? GetInterpolation(Vector3 point, Triangle triangle)
    {
        var vec3Barycoord = GetBarycoord(point);
        
        return vec3Barycoord.HasValue ? Vector3.Zero.Add(triangle.v1.MultiplyScalar(vec3Barycoord.Value.X))
            .Add(triangle.v2.MultiplyScalar(vec3Barycoord.Value.Y))
            .Add(triangle.v3.MultiplyScalar(vec3Barycoord.Value.Z)) : null;
    }

    public bool Equals(Triangle other)
    {
        return v1.Equals(other.v1) && v2.Equals(other.v2) && v3.Equals(other.v3);
    }

    public override bool Equals(object obj)
    {
        return obj is Triangle other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(v1.GetHashCode(), v2.GetHashCode(), v3.GetHashCode());
    }

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format) => ToString(format, CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format, IFormatProvider formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return
            $"triangle<{v1.ToString(format, formatProvider)}{separator} {v2.ToString(format, formatProvider)}{separator} {v3.ToString(format, formatProvider)}>";
    }
}