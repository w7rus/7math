using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace _7math;

public struct Matrix3 : IEquatable<Matrix3>, IFormattable
{
    public float[] Elements { get; set; }

    public Matrix3() =>
        Elements = new float[]
        {
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Set(
        float n11 = 0,
        float n12 = 0,
        float n13 = 0,
        float n21 = 0,
        float n22 = 0,
        float n23 = 0,
        float n31 = 0,
        float n32 = 0,
        float n33 = 0
    )
    {
        Elements[0] = n11;
        Elements[1] = n21;
        Elements[2] = n31;
        Elements[3] = n12;
        Elements[4] = n22;
        Elements[5] = n32;
        Elements[6] = n13;
        Elements[7] = n23;
        Elements[8] = n33;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Clone() => new Matrix3().Copy(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Copy(Matrix3 mat3) => SetFromArray(mat3.Elements);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 SetFromArray(float[] arr, int offset = 0)
    {
        for (var i = 0; i < 8; i++) Elements[i] = arr[i + offset];

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float[] ToArray() => (float[])Elements.Clone();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 SetIdentity() =>
        Set(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (Vector3 vec3x, Vector3 vec3y, Vector3 vec3z) GetBasis() =>
    (
        new Vector3().SetFromMatrix3Column(this, 0),
        new Vector3().SetFromMatrix3Column(this, 1),
        new Vector3().SetFromMatrix3Column(this, 2)
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 SetFromMatrix4(Matrix4 mat4) =>
        Set(
            mat4.Elements[0], mat4.Elements[4], mat4.Elements[8],
            mat4.Elements[1], mat4.Elements[5], mat4.Elements[9],
            mat4.Elements[2], mat4.Elements[6], mat4.Elements[10]
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Multiply(Matrix3 mat3)
    {
        var a11 = Elements[0];
        var a12 = Elements[3];
        var a13 = Elements[6];
        var a21 = Elements[1];
        var a22 = Elements[4];
        var a23 = Elements[7];
        var a31 = Elements[2];
        var a32 = Elements[5];
        var a33 = Elements[8];
        var b11 = mat3.Elements[0];
        var b12 = mat3.Elements[3];
        var b13 = mat3.Elements[6];
        var b21 = mat3.Elements[1];
        var b22 = mat3.Elements[4];
        var b23 = mat3.Elements[7];
        var b31 = mat3.Elements[2];
        var b32 = mat3.Elements[5];
        var b33 = mat3.Elements[8];

        Elements[0] = a11 * b11 + a12 * b21 + a13 * b31;
        Elements[3] = a11 * b12 + a12 * b22 + a13 * b32;
        Elements[6] = a11 * b13 + a12 * b23 + a13 * b33;
        Elements[1] = a21 * b11 + a22 * b21 + a23 * b31;
        Elements[4] = a21 * b12 + a22 * b22 + a23 * b32;
        Elements[7] = a21 * b13 + a22 * b23 + a23 * b33;
        Elements[2] = a31 * b11 + a32 * b21 + a33 * b31;
        Elements[5] = a31 * b12 + a32 * b22 + a33 * b32;
        Elements[8] = a31 * b13 + a32 * b23 + a33 * b33;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Premultiply(Matrix3 mat3)
    {
        var a11 = mat3.Elements[0];
        var a12 = mat3.Elements[3];
        var a13 = mat3.Elements[6];
        var a21 = mat3.Elements[1];
        var a22 = mat3.Elements[4];
        var a23 = mat3.Elements[7];
        var a31 = mat3.Elements[2];
        var a32 = mat3.Elements[5];
        var a33 = mat3.Elements[8];
        var b11 = Elements[0];
        var b12 = Elements[3];
        var b13 = Elements[6];
        var b21 = Elements[1];
        var b22 = Elements[4];
        var b23 = Elements[7];
        var b31 = Elements[2];
        var b32 = Elements[5];
        var b33 = Elements[8];

        Elements[0] = a11 * b11 + a12 * b21 + a13 * b31;
        Elements[3] = a11 * b12 + a12 * b22 + a13 * b32;
        Elements[6] = a11 * b13 + a12 * b23 + a13 * b33;
        Elements[1] = a21 * b11 + a22 * b21 + a23 * b31;
        Elements[4] = a21 * b12 + a22 * b22 + a23 * b32;
        Elements[7] = a21 * b13 + a22 * b23 + a23 * b33;
        Elements[2] = a31 * b11 + a32 * b21 + a33 * b31;
        Elements[5] = a31 * b12 + a32 * b22 + a33 * b32;
        Elements[8] = a31 * b13 + a32 * b23 + a33 * b33;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 MultiplyScalar(float scalar)
    {
        Elements[0] *= scalar;
        Elements[3] *= scalar;
        Elements[6] *= scalar;
        Elements[1] *= scalar;
        Elements[4] *= scalar;
        Elements[7] *= scalar;
        Elements[2] *= scalar;
        Elements[5] *= scalar;
        Elements[8] *= scalar;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Determinant()
    {
        var a = Elements[0];
        var b = Elements[1];
        var c = Elements[2];
        var d = Elements[3];
        var e = Elements[4];
        var f = Elements[5];
        var g = Elements[6];
        var h = Elements[7];
        var i = Elements[8];

        return a * e * i - a * f * h - b * d * i + b * f * g + c * d * h - c * e * g;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Invert()
    {
        var n11 = Elements[0];
        var n21 = Elements[1];
        var n31 = Elements[2];
        var n12 = Elements[3];
        var n22 = Elements[4];
        var n32 = Elements[5];
        var n13 = Elements[6];
        var n23 = Elements[7];
        var n33 = Elements[8];

        var t11 = n33 * n22 - n32 * n23;
        var t12 = n32 * n13 - n33 * n12;
        var t13 = n23 * n12 - n22 * n13;

        var determinant = n11 * t11 + n21 * t12 + n31 * t13;

        if (determinant == 0)
            return Set();

        var inverseDeterminant = 1 / determinant;

        Elements[0] = t11 * inverseDeterminant;
        Elements[1] = (n31 * n23 - n33 * n21) * inverseDeterminant;
        Elements[2] = (n32 * n21 - n31 * n22) * inverseDeterminant;
        Elements[3] = t12 * inverseDeterminant;
        Elements[4] = (n33 * n11 - n31 * n13) * inverseDeterminant;
        Elements[5] = (n31 * n12 - n32 * n11) * inverseDeterminant;
        Elements[6] = t13 * inverseDeterminant;
        Elements[7] = (n21 * n13 - n23 * n11) * inverseDeterminant;
        Elements[8] = (n22 * n11 - n21 * n12) * inverseDeterminant;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Transpose()
    {
        var tmp = Elements[1];
        Elements[1] = Elements[3];
        Elements[3] = tmp;
        tmp = Elements[2];
        Elements[2] = Elements[6];
        Elements[6] = tmp;
        tmp = Elements[5];
        Elements[5] = Elements[7];
        Elements[7] = tmp;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 GetNormalMatrix(Matrix4 mat4) => SetFromMatrix4(mat4).Invert().Transpose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 MakeScale(float x, float y) =>
        Set(
            x, 0, 0,
            0, y, 0,
            0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 MakeRotation(float theta)
    {
        var nCosTheta = MathF.Cos(theta);
        var nSinTheta = MathF.Sin(theta);

        return Set(
            nCosTheta, -nSinTheta, 0,
            nSinTheta, nCosTheta, 0,
            0, 0, 1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 MakeTranslation(float x, float y) =>
        Set(
            1, 0, x,
            0, 1, y,
            0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 MakeShear(float x, float y) =>
        Set(
            1, y, 0,
            x, 1, 0,
            x, y, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Scale(float x, float y) => Premultiply(new Matrix3().MakeScale(x, y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Rotate(float theta) => Premultiply(new Matrix3().MakeRotation(theta));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Translate(float x, float y) => Premultiply(new Matrix3().MakeTranslation(x, y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3 Shear(float x, float y) => Premultiply(new Matrix3().MakeShear(x, y));

    public bool Equals(Matrix3 other) => Elements.SequenceEqual(other.Elements);

    public override bool Equals(object obj) => obj is Matrix3 other && Equals(other);

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format) => ToString(format, CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format, IFormatProvider formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return
            $"""
             mat3<
                {Elements[0].ToString(format, formatProvider)}{separator} {Elements[1].ToString(format, formatProvider)}{separator} {Elements[2].ToString(format, formatProvider)}
                {Elements[3].ToString(format, formatProvider)}{separator} {Elements[4].ToString(format, formatProvider)}{separator} {Elements[5].ToString(format, formatProvider)}
                {Elements[6].ToString(format, formatProvider)}{separator} {Elements[7].ToString(format, formatProvider)}{separator} {Elements[8].ToString(format, formatProvider)}
             >
             """;
    }

    public override int GetHashCode() =>
        Elements is { Length: 9 }
            ? HashCode.Combine(
                HashCode.Combine(Elements[0], Elements[1], Elements[2]),
                HashCode.Combine(Elements[3], Elements[4], Elements[5]),
                HashCode.Combine(Elements[6], Elements[7], Elements[8])
            )
            : 0;
}