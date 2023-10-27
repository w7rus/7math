using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace _7math;

public struct Vector3 : IEquatable<Vector3>, IFormattable
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3(float x = 0, float y = 0, float z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3 Zero => new();
    public static Vector3 One => new(1, 1, 1);
    public static Vector3 UnitX => new(1);
    public static Vector3 UnitY => new(y: 1);
    public static Vector3 UnitZ => new(z: 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Clone()
    {
        return new Vector3(X, Y, Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Copy(Vector3 vec3)
    {
        X = vec3.X;
        Y = vec3.Y;
        Z = vec3.Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Set(float x = 0, float y = 0, float z = 0)
    {
        X = x;
        Y = y;
        Z = z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Add(Vector3 vec3)
    {
        X += vec3.X;
        Y += vec3.Y;
        Z += vec3.Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 AddScalar(float scalar)
    {
        X += scalar;
        Y += scalar;
        Z += scalar;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ApplyAxisAngle(Vector3 vec3, float theta) => ApplyQuaternion(new Quaternion().SetFromAxisAngle(vec3, theta));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ApplyEuler(Euler eul) => ApplyQuaternion(new Quaternion().SetFromEuler(eul));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ApplyMatrix3(Matrix3 mat3)
    {
        var nX = X;
        var nY = Y;
        var nZ = Z;

        X = mat3.Elements[0] * nX + mat3.Elements[3] * nY + mat3.Elements[6] * nZ;
        Y = mat3.Elements[1] * nX + mat3.Elements[4] * nY + mat3.Elements[7] * nZ;
        Z = mat3.Elements[2] * nX + mat3.Elements[5] * nY + mat3.Elements[8] * nZ;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ApplyMatrix4(Matrix4 mat4)
    {
        var nX = X;
        var nY = Y;
        var nZ = Z;
        var nW = 1 / (mat4.Elements[3] * nX + mat4.Elements[7] * nY + mat4.Elements[11] * nZ + mat4.Elements[15]);

        X = (mat4.Elements[0] * nX + mat4.Elements[4] * nY + mat4.Elements[8] * nZ + mat4.Elements[12]) * nW;
        Y = (mat4.Elements[1] * nX + mat4.Elements[5] * nY + mat4.Elements[9] * nZ + mat4.Elements[13]) * nW;
        Z = (mat4.Elements[2] * nX + mat4.Elements[6] * nY + mat4.Elements[10] * nZ + mat4.Elements[14]) * nW;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ApplyQuaternion(Quaternion quat)
    {
        var nX1 = X;
        var nY1 = Y;
        var nZ1 = Z;

        var nX2 = quat.X;
        var nY2 = quat.Y;
        var nZ2 = quat.Z;
        var nW2 = quat.W;

        var nX3 = nW2 * nX1 + nY2 * nZ1 - nZ2 * nY1;
        var nY3 = nW2 * nY1 + nZ2 * nX1 - nX2 * nZ1;
        var nZ3 = nW2 * nZ1 + nX2 * nY1 - nY2 * nX1;
        var nW3 = -nX2 * nX1 - nY2 * nY1 - nZ2 * nZ1;

        X = nX3 * nW2 + nW3 * -nX2 + nY3 * -nZ2 - nZ3 * -nY2;
        Y = nY3 * nW2 + nW3 * -nY2 + nZ3 * -nX2 - nX3 * -nZ2;
        Z = nZ3 * nW2 + nW3 * -nZ2 + nX3 * -nY2 - nY3 * -nX2;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float AngleTo(Vector3 vec3)
    {
        var denominator = vec3.LengthSquared();

        return denominator == 0 ? MathF.PI / 2 : MathF.Acos(Helpers.Clamp(Dot(vec3) / denominator, -1, 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Clamp(Vector3 min, Vector3 max)
    {
        X = MathF.Max(min.X, MathF.Max(max.X, X));
        Y = MathF.Max(min.Y, MathF.Max(max.Y, Y));
        Z = MathF.Max(min.Z, MathF.Max(max.Z, Z));

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ClampLength(float min, float max)
    {
        var length = Length();

        DivideScalar(length == 0 ? 1 : length).MultiplyScalar(MathF.Max(min, MathF.Min(max, length)));

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ClampScalar(float min, float max)
    {
        X = MathF.Max(min, MathF.Max(max, X));
        Y = MathF.Max(min, MathF.Max(max, Y));
        Z = MathF.Max(min, MathF.Max(max, Z));

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Cross(Vector3 vec3)
    {
        var nX = X;
        var nY = Y;
        var nZ = Z;

        X = nY * vec3.Z - nZ * vec3.Y;
        Y = nZ * vec3.X - nX * vec3.Z;
        Z = nX * vec3.Y - nY * vec3.X;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float DistanceTo(Vector3 vec3)
    {
        return MathF.Sqrt(DistanceToSquared(vec3));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float DistanceToSquared(Vector3 vec3)
    {
        var x = X - vec3.X;
        var y = Y - vec3.Y;
        var z = Z - vec3.Z;

        return x * x + y * y + z * z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Divide(Vector3 vec3)
    {
        X /= vec3.X;
        Y /= vec3.Y;
        Z /= vec3.Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 DivideScalar(float scalar) => MultiplyScalar(1 / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Dot(Vector3 vec3) => X * vec3.X + Y * vec3.Y + Z * vec3.Z;

    public bool Equals(Vector3 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

    public override bool Equals(object obj) => obj is Vector3 other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Length() => MathF.Sqrt(LengthSquared());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float LengthSquared() => Dot(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Lerp(Vector3 vec3, float alpha)
    {
        X += (vec3.X - X) * alpha;
        Y += (vec3.Y - Y) * alpha;
        Z += (vec3.Z - Z) * alpha;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Max(Vector3 vec3)
    {
        X = MathF.Max(vec3.X, X);
        Y = MathF.Max(vec3.Y, Y);
        Z = MathF.Max(vec3.Z, Z);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Min(Vector3 vec3)
    {
        X = MathF.Min(vec3.X, X);
        Y = MathF.Min(vec3.Y, Y);
        Z = MathF.Min(vec3.Z, Z);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Multiply(Vector3 vec3)
    {
        X *= vec3.X;
        Y *= vec3.Y;
        Z *= vec3.Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 MultiplyScalar(float scalar)
    {
        X *= scalar;
        Y *= scalar;
        Z *= scalar;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Negate()
    {
        X = -X;
        Y = -Y;
        Z = -Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Normalize()
    {
        var length = Length();

        return DivideScalar(length == 0 ? 1 : length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Project(Camera camera)
    {
        throw new NotImplementedException();
        // ApplyMatrix4(camera.MatrixWorldInverse);
        // ApplyMatrix4(camera.ProjectionMatrix);
        //
        // return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ToScreen(Camera camera, Vector3 vec3Viewport)
    {
        throw new NotImplementedException();
        // Project(camera);
        // X = (X * vec3Viewport.X / 2) + vec3Viewport.X / 2;
        // Y = -(Y * vec3Viewport.Y / 2) + vec3Viewport.Y / 2;
        //
        // return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ProjectOnPlane(Vector3 normal) => Subtract(Clone().ProjectOnVector(normal));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ProjectOnVector(Vector3 vec3)
    {
        var denominator = vec3.LengthSquared();

        return denominator == 0 ? Set() : vec3.MultiplyScalar(vec3.Dot(this) / denominator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Reflect(Vector3 normal) => Subtract(normal.MultiplyScalar(2 * Dot(normal)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 SetFromArray(float[] arr, int offset = 0)
    {
        X = arr[offset];
        Y = arr[offset + 1];
        Z = arr[offset + 2];

        return this;
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public void SetFromCylindrical(Cylindrical cylindrical)
    // {
    //     throw new NotImplementedException();
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 SetFromMatrix4Column(Matrix4 mat4, int columnIndex) => SetFromArray(mat4.Elements, columnIndex * 4);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 SetFromMatrix3Column(Matrix3 mat3, int columnIndex) => SetFromArray(mat3.Elements, columnIndex * 3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 SetFromMatrix4Position(Matrix4 mat4)
    {
        X = mat4.Elements[12];
        Y = mat4.Elements[13];
        Z = mat4.Elements[14];

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 SetFromMatrix4Scale(Matrix4 mat4)
    {
        X = SetFromMatrix4Column(mat4, 0).Length();
        Y = SetFromMatrix4Column(mat4, 1).Length();
        Z = SetFromMatrix4Column(mat4, 2).Length();

        return this;
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public void SetFromSpherical(Spherical spherical)
    // {
    //     throw new NotImplementedException();
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ScaleTo(float length) => Normalize().MultiplyScalar(length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 SetFromScalar(float scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Subtract(Vector3 vec3)
    {
        X -= vec3.X;
        Y -= vec3.Y;
        Z -= vec3.Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 SubtractScalar(float scalar)
    {
        X -= scalar;
        Y -= scalar;
        Z -= scalar;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float[] ToArray() => new[] { X, Y, Z };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 TransformDirection(Matrix4 mat4)
    {
        var nX = X;
        var nY = Y;
        var nZ = Z;

        X = mat4.Elements[0] * nX + mat4.Elements[4] * nY + mat4.Elements[8] * nZ;
        Y = mat4.Elements[1] * nX + mat4.Elements[5] * nY + mat4.Elements[9] * nZ;
        Z = mat4.Elements[2] * nX + mat4.Elements[6] * nY + mat4.Elements[10] * nZ;

        return Normalize();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Unproject(Camera camera)
    {
        throw new NotImplementedException();
        // return ApplyMatrix4(camera.ProjectionMatrixInverse).ApplyMatrix4(camera.MatrixWorld);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Unscreen(Camera camera, Vector3 vec3Viewport)
    {
        throw new NotImplementedException();
        // X = ((X - vec3Viewport.X / 2) / (vec3Viewport.X / 2));
        // Y = -((Y - vec3Viewport.Y / 2) / (vec3Viewport.Y / 2));
        // return Unproject(camera);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFinite() => float.IsFinite(X) && float.IsFinite(Y) && float.IsFinite(Z);

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format) => ToString(format, CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format, IFormatProvider formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return $"vec3<{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}{separator} {Z.ToString(format, formatProvider)}>";
    }

    public static bool operator ==(Vector3 left, Vector3 right) => left.Equals(right);

    public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);
}