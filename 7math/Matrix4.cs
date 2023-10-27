using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace _7math;

public struct Matrix4 : IEquatable<Matrix4>, IFormattable
{
    public float[] Elements { get; set; }

    public Matrix4() =>
        Elements = new float[]
        {
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Set(
        float n11 = 0,
        float n12 = 0,
        float n13 = 0,
        float n14 = 0,
        float n21 = 0,
        float n22 = 0,
        float n23 = 0,
        float n24 = 0,
        float n31 = 0,
        float n32 = 0,
        float n33 = 0,
        float n34 = 0,
        float n41 = 0,
        float n42 = 0,
        float n43 = 0,
        float n44 = 0
    )
    {
        Elements[0] = n11;
        Elements[1] = n21;
        Elements[2] = n31;
        Elements[3] = n41;
        Elements[4] = n12;
        Elements[5] = n22;
        Elements[6] = n32;
        Elements[7] = n42;
        Elements[8] = n13;
        Elements[9] = n23;
        Elements[10] = n33;
        Elements[11] = n43;
        Elements[12] = n14;
        Elements[13] = n24;
        Elements[14] = n34;
        Elements[15] = n44;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Clone() => new Matrix4().Copy(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Compose(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        var nX1 = rotation.X;
        var nY1 = rotation.Y;
        var nZ1 = rotation.Z;
        var nW1 = rotation.W;

        var nX2 = nX1 + nX1;
        var nY2 = nY1 + nY1;
        var nZ2 = nZ1 + nZ1;

        var nXx = nX1 * nX2;
        var nXy = nX1 * nY2;
        var nXz = nX1 * nZ2;

        var nYy = nY1 * nY2;
        var nYz = nY1 * nZ2;
        var nZz = nZ1 * nZ2;

        var nWx = nW1 * nX2;
        var nWy = nW1 * nY2;
        var nWz = nW1 * nZ2;

        var nSx = scale.X;
        var nSy = scale.Y;
        var nSz = scale.Z;

        Elements[0] = (1 - (nYy + nZz)) * nSx;
        Elements[1] = (nXy + nWz) * nSx;
        Elements[2] = (nXz - nWy) * nSx;
        Elements[3] = 0;
        Elements[4] = (nXy - nWz) * nSy;
        Elements[5] = (1 - (nXx + nZz)) * nSy;
        Elements[6] = (nYz + nWx) * nSy;
        Elements[7] = 0;
        Elements[8] = (nXz + nWy) * nSz;
        Elements[9] = (nYz - nWx) * nSz;
        Elements[10] = (1 - (nXx + nYy)) * nSz;
        Elements[11] = 0;
        Elements[12] = position.X;
        Elements[13] = position.Y;
        Elements[14] = position.Z;
        Elements[15] = 1;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Copy(Matrix4 mat4) => SetFromArray(mat4.Elements);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 SetFromArray(float[] arr, int offset = 0)
    {
        for (var i = 0; i < 15; i++) Elements[i] = arr[i + offset];

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 CopyPositionFromMatrix4(Matrix4 mat4)
    {
        Elements[12] = mat4.Elements[12];
        Elements[13] = mat4.Elements[13];
        Elements[14] = mat4.Elements[14];

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (Vector3 position, Quaternion rotation, Vector3 scale) GetDecomposition()
    {
        var nSx = new Vector3(Elements[0], Elements[1], Elements[2]).Length();
        var nSy = new Vector3(Elements[4], Elements[5], Elements[6]).Length();
        var nSz = new Vector3(Elements[8], Elements[9], Elements[10]).Length();

        if (Determinant() < 0)
            nSx = -nSx;

        var position = new Vector3
        {
            X = Elements[12],
            Y = Elements[13],
            Z = Elements[14]
        };

        var mat4 = Clone();
        var inverseSx = 1 / nSx;
        var inverseSy = 1 / nSy;
        var inverseSz = 1 / nSz;
        mat4.Elements[0] *= inverseSx;
        mat4.Elements[1] *= inverseSx;
        mat4.Elements[2] *= inverseSx;
        mat4.Elements[4] *= inverseSy;
        mat4.Elements[5] *= inverseSy;
        mat4.Elements[6] *= inverseSy;
        mat4.Elements[8] *= inverseSz;
        mat4.Elements[9] *= inverseSz;
        mat4.Elements[10] *= inverseSz;

        var rotation = new Quaternion().SetFromRotationMatrix(mat4);
        var scale = new Vector3
        {
            X = nSx,
            Y = nSy,
            Z = nSz
        };

        return (position, rotation, scale);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Determinant()
    {
        var n11 = Elements[0];
        var n12 = Elements[4];
        var n13 = Elements[8];
        var n14 = Elements[12];
        var n21 = Elements[1];
        var n22 = Elements[5];
        var n23 = Elements[9];
        var n24 = Elements[13];
        var n31 = Elements[2];
        var n32 = Elements[6];
        var n33 = Elements[10];
        var n34 = Elements[14];
        var n41 = Elements[3];
        var n42 = Elements[7];
        var n43 = Elements[11];
        var n44 = Elements[15];

        return n41 * (
                   n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34
               ) +
               n42 * (
                   n11 * n23 * n34 - n11 * n24 * n33 + n14 * n21 * n33 - n13 * n21 * n34 + n13 * n24 * n31 - n14 * n23 * n31
               ) +
               n43 * (
                   n11 * n24 * n32 - n11 * n22 * n34 - n14 * n21 * n32 + n12 * n21 * n34 + n14 * n22 * n31 - n12 * n24 * n31
               ) +
               n44 * (
                   -(n13 * n22 * n31) - n11 * n23 * n32 + n11 * n22 * n33 + n13 * n21 * n32 - n12 * n21 * n33 + n12 * n23 * n31
               );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (Vector3 vec3x, Vector3 vec3y, Vector3 vec3z) GetBasis() =>
    (
        new Vector3().SetFromMatrix4Column(this, 0),
        new Vector3().SetFromMatrix4Column(this, 1),
        new Vector3().SetFromMatrix4Column(this, 2)
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 GetRotation(Matrix4 mat4)
    {
        var scaleX = 1 / new Vector3().SetFromMatrix4Column(this, 0).Length();
        var scaleY = 1 / new Vector3().SetFromMatrix4Column(this, 1).Length();
        var scaleZ = 1 / new Vector3().SetFromMatrix4Column(this, 2).Length();

        return mat4.Set(
            mat4.Elements[0] * scaleX, mat4.Elements[1] * scaleX, mat4.Elements[2] * scaleX, 0,
            mat4.Elements[4] * scaleY, mat4.Elements[5] * scaleY, mat4.Elements[6] * scaleY, 0,
            mat4.Elements[8] * scaleZ, mat4.Elements[9] * scaleZ, mat4.Elements[10] * scaleZ, 0,
            0, 0, 0, 1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Invert()
    {
        var n11 = Elements[0];
        var n21 = Elements[1];
        var n31 = Elements[2];
        var n41 = Elements[3];
        var n12 = Elements[4];
        var n22 = Elements[5];
        var n32 = Elements[6];
        var n42 = Elements[7];
        var n13 = Elements[8];
        var n23 = Elements[9];
        var n33 = Elements[10];
        var n43 = Elements[11];
        var n14 = Elements[12];
        var n24 = Elements[13];
        var n34 = Elements[14];
        var n44 = Elements[15];

        var t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
        var t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
        var t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
        var t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

        var determinant = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;

        if (determinant == 0)
            return Set();

        var inverseDeterminant = 1 / determinant;

        Elements[0] = t11 * inverseDeterminant;
        Elements[1] = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * inverseDeterminant;
        Elements[2] = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * inverseDeterminant;
        Elements[3] = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * inverseDeterminant;
        Elements[4] = t12 * inverseDeterminant;
        Elements[5] = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * inverseDeterminant;
        Elements[6] = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * inverseDeterminant;
        Elements[7] = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * inverseDeterminant;
        Elements[8] = t13 * inverseDeterminant;
        Elements[9] = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * inverseDeterminant;
        Elements[10] = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * inverseDeterminant;
        Elements[11] = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * inverseDeterminant;
        Elements[12] = t14 * inverseDeterminant;
        Elements[13] = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * inverseDeterminant;
        Elements[14] = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * inverseDeterminant;
        Elements[15] = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * inverseDeterminant;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float MaxScaleOnAxis()
    {
        var scaleXSquared = Elements[0] * Elements[0] + Elements[1] * Elements[1] + Elements[2] * Elements[2];
        var scaleYSquared = Elements[4] * Elements[4] + Elements[5] * Elements[5] + Elements[6] * Elements[6];
        var scaleZSquared = Elements[8] * Elements[8] + Elements[9] * Elements[9] + Elements[10] * Elements[10];

        return MathF.Sqrt(MathF.Max(scaleXSquared, MathF.Max(scaleYSquared, scaleZSquared)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 SetIdentity() =>
        Set(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeLookAt(Vector3 position, Vector3 target, Vector3 up)
    {
        var vec3Z = new Vector3().Copy(position).Subtract(target);

        if (vec3Z.LengthSquared() == 0) vec3Z.Z = 1;

        vec3Z.Normalize();
        var vec3X = new Vector3().Copy(up).Cross(vec3Z);

        if (vec3X.LengthSquared() == 0)
        {
            if (MathF.Abs(up.Z) == 1)
                vec3Z.X += float.Epsilon;
            else
                vec3Z.Z += float.Epsilon;
        }

        vec3X.Normalize();
        var vec3Y = new Vector3().Copy(up).Cross(vec3X);

        Elements[0] = vec3X.X;
        Elements[4] = vec3Y.X;
        Elements[8] = vec3Z.X;
        Elements[1] = vec3X.Y;
        Elements[5] = vec3Y.Y;
        Elements[9] = vec3Z.Y;
        Elements[2] = vec3X.Z;
        Elements[6] = vec3Y.Z;
        Elements[10] = vec3Z.Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeRotationFromAxisAngle(Vector3 vec3, float theta)
    {
        var nCosTheta = MathF.Cos(theta);
        var nSinTheta = MathF.Sin(theta);
        var nT = 1 - nCosTheta;
        var nX = vec3.X;
        var nY = vec3.Y;
        var nZ = vec3.Z;
        var nTx = nT * nX;
        var nTy = nT * nY;

        return Set(
            nTx * nX + nCosTheta, nTx * nY - nSinTheta * nZ, nTx * nZ + nSinTheta * nY, 0,
            nTx * nY + nSinTheta * nZ, nTy * nY + nCosTheta, nTy * nZ - nSinTheta * nX, 0,
            nTx * nZ - nSinTheta * nY, nTy * nZ + nSinTheta * nX, nT * nZ * nZ + nCosTheta, 0,
            0, 0, 0, 1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeBasis(Vector3 vec3X, Vector3 vec3Y, Vector3 vec3Z) =>
        Set(
            vec3X.X, vec3Y.X, vec3Z.X, 0,
            vec3X.Y, vec3Y.Y, vec3Z.Y, 0,
            vec3X.Z, vec3Y.Z, vec3Z.Z, 0,
            0, 0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakePerspective(float left, float right, float top, float bottom, float near, float far)
    {
        var nW = right - left;
        var nH = top - bottom;
        var nP = far - near;

        return Set(
            2 * near / nW, 0, (right + left) / nW, 0,
            0, 2 * near / nH, (top + bottom) / nH, 0,
            0, 0, -(far + near) / nP, -2 * far * near / nP,
            0, 0, -1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeOrthographic(float left, float right, float top, float bottom, float near, float far)
    {
        var nW = right - left;
        var nH = top - bottom;
        var nP = far - near;

        return Set(
            2 / nW, 0, 0, -(right + left) * nW,
            0, 2 / nH, 0, -(top + bottom) * nH,
            0, 0, -2 / nP, -(far + near) * nP,
            0, 0, 0, 1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeRotationFromEuler(Euler eul)
    {
        var nX = eul.X;
        var nY = eul.Y;
        var nZ = eul.Z;
        var order = eul.Order;
        var nCosX = MathF.Cos(nX);
        var nCosY = MathF.Cos(nY);
        var nCosZ = MathF.Cos(nZ);
        var nSinX = MathF.Sin(nX);
        var nSinY = MathF.Sin(nY);
        var nSinZ = MathF.Sin(nZ);

        switch (order)
        {
            case AxisOrder.XYZ:
            {
                var nCosXCosZ = nCosX * nCosZ;
                var nCosXSinZ = nCosX * nSinZ;
                var nSinXCosZ = nSinX * nCosZ;
                var nSinXSinZ = nSinX * nSinZ;
                Elements[0] = nCosY * nCosZ;
                Elements[4] = -(nCosY * nSinZ);
                Elements[8] = nSinY;
                Elements[1] = nCosXSinZ + nSinXCosZ * nSinY;
                Elements[5] = nCosXCosZ - nSinXSinZ * nSinY;
                Elements[9] = -(nSinX * nCosY);
                Elements[2] = nSinXSinZ - nCosXCosZ * nSinY;
                Elements[6] = nSinXCosZ + nCosXSinZ * nSinY;
                Elements[10] = nCosX * nCosY;
                break;
            }
            case AxisOrder.YXZ:
            {
                var nCosYCosZ = nCosY * nCosZ;
                var nCosYSinZ = nCosY * nSinZ;
                var nSinYCosZ = nSinY * nCosZ;
                var nSinYSinZ = nSinY * nSinZ;
                Elements[0] = nCosYCosZ + nSinYSinZ * nSinX;
                Elements[4] = nSinYCosZ * nSinX - nCosYSinZ;
                Elements[8] = nCosX * nSinY;
                Elements[1] = nCosX * nSinZ;
                Elements[5] = nCosX * nCosZ;
                Elements[9] = -nSinX;
                Elements[2] = nCosYSinZ * nSinX - nSinYCosZ;
                Elements[6] = nSinYSinZ + nCosYCosZ * nSinX;
                Elements[10] = nCosX * nCosY;
                break;
            }
            case AxisOrder.ZXY:
            {
                var nCosYCosZ = nCosY * nCosZ;
                var nCosYSinZ = nCosY * nSinZ;
                var nSinYCosZ = nSinY * nCosZ;
                var nSinYSinZ = nSinY * nSinZ;
                Elements[0] = nCosYCosZ - nSinYSinZ * nSinX;
                Elements[4] = -(nCosX * nSinZ);
                Elements[8] = nSinYCosZ + nCosYSinZ * nSinX;
                Elements[1] = nCosYSinZ + nSinYCosZ * nSinX;
                Elements[5] = nCosX * nCosZ;
                Elements[9] = nSinYSinZ - nCosYCosZ * nSinX;
                Elements[2] = -(nCosX * nSinY);
                Elements[6] = nSinX;
                Elements[10] = nCosX * nCosY;
                break;
            }
            case AxisOrder.ZYX:
            {
                var nCosXCosZ = nCosX * nCosZ;
                var nCosXSinZ = nCosX * nSinZ;
                var nSinXCosZ = nSinX * nCosZ;
                var nSinXSinZ = nSinX * nSinZ;
                Elements[0] = nCosY * nCosZ;
                Elements[4] = nSinXCosZ * nSinY - nCosXSinZ;
                Elements[8] = nCosXCosZ * nSinY + nSinXSinZ;
                Elements[1] = nCosY * nSinZ;
                Elements[5] = nSinXSinZ * nSinY + nCosXCosZ;
                Elements[9] = nCosXSinZ * nSinY - nSinXCosZ;
                Elements[2] = -nSinY;
                Elements[6] = nSinX * nCosY;
                Elements[10] = nCosX * nCosY;
                break;
            }
            case AxisOrder.YZX:
            {
                var nCosXCosY = nCosX * nCosY;
                var nCosXSinY = nCosX * nSinY;
                var nSinXCosY = nSinX * nCosY;
                var nSinXSinY = nSinX * nSinY;
                Elements[0] = nCosY * nCosZ;
                Elements[4] = nSinXSinY - nCosXCosY * nSinZ;
                Elements[8] = nSinXCosY * nSinZ + nCosXSinY;
                Elements[1] = nSinZ;
                Elements[5] = nCosX * nCosZ;
                Elements[9] = -(nSinX * nCosZ);
                Elements[2] = -(nSinY * nCosZ);
                Elements[6] = nCosXSinY * nSinZ + nSinXCosY;
                Elements[10] = nCosXCosY - nSinXSinY * nSinZ;
                break;
            }
            case AxisOrder.XZY:
            {
                var nCosXCosY = nCosX * nCosY;
                var nCosXSinY = nCosX * nSinY;
                var nSinXCosY = nSinX * nCosY;
                var nSinXSinY = nSinX * nSinY;
                Elements[0] = nCosY * nCosZ;
                Elements[4] = -nSinZ;
                Elements[8] = nSinY * nCosZ;
                Elements[1] = nCosXCosY * nSinZ + nSinXSinY;
                Elements[5] = nCosX * nCosZ;
                Elements[9] = nCosXSinY * nSinZ - nSinXCosY;
                Elements[2] = nSinXCosY * nSinZ - nCosXSinY;
                Elements[6] = nSinX * nCosZ;
                Elements[10] = nSinXSinY * nSinZ + nCosXCosY;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeRotationFromQuaternion(Quaternion quat) => Compose(Vector3.Zero, quat, Vector3.One);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeRotationX(float theta)
    {
        var nCosTheta = MathF.Cos(theta);
        var nSinTheta = MathF.Sin(theta);

        return Set(
            1, 0, 0, 0,
            0, nCosTheta, -nSinTheta, 0,
            0, nSinTheta, nCosTheta, 0,
            0, 0, 0, 1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeRotationY(float theta)
    {
        var nCosTheta = MathF.Cos(theta);
        var nSinTheta = MathF.Sin(theta);

        return Set(
            nCosTheta, 0, nSinTheta, 0,
            0, 1, 0, 0,
            -nSinTheta, 0, nCosTheta, 0,
            0, 0, 0, 1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeRotationZ(float theta)
    {
        var nCosTheta = MathF.Cos(theta);
        var nSinTheta = MathF.Sin(theta);

        return Set(
            nCosTheta, -nSinTheta, 0, 0,
            nSinTheta, nCosTheta, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeScale(Vector3 vec3) =>
        Set(
            vec3.X, 0, 0, 0,
            0, vec3.Y, 0, 0,
            0, 0, vec3.Z, 0,
            0, 0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeShear(Vector3 vec3) =>
        Set(
            1, vec3.Y, vec3.Z, 0,
            vec3.X, 1, vec3.Z, 0,
            vec3.X, vec3.Y, 1, 0,
            0, 0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MakeTranslation(Vector3 vec3) =>
        Set(
            1, 0, 0, vec3.X,
            0, 1, 0, vec3.Y,
            0, 0, 1, vec3.Z,
            0, 0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Multiply(Matrix4 mat4)
    {
        var a11 = Elements[0];
        var a12 = Elements[4];
        var a13 = Elements[8];
        var a14 = Elements[12];
        var a21 = Elements[1];
        var a22 = Elements[5];
        var a23 = Elements[9];
        var a24 = Elements[13];
        var a31 = Elements[2];
        var a32 = Elements[6];
        var a33 = Elements[10];
        var a34 = Elements[14];
        var a41 = Elements[3];
        var a42 = Elements[7];
        var a43 = Elements[11];
        var a44 = Elements[15];
        var b11 = mat4.Elements[0];
        var b12 = mat4.Elements[4];
        var b13 = mat4.Elements[8];
        var b14 = mat4.Elements[12];
        var b21 = mat4.Elements[1];
        var b22 = mat4.Elements[5];
        var b23 = mat4.Elements[9];
        var b24 = mat4.Elements[13];
        var b31 = mat4.Elements[2];
        var b32 = mat4.Elements[6];
        var b33 = mat4.Elements[10];
        var b34 = mat4.Elements[14];
        var b41 = mat4.Elements[3];
        var b42 = mat4.Elements[7];
        var b43 = mat4.Elements[11];
        var b44 = mat4.Elements[15];

        Elements[0] = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
        Elements[4] = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
        Elements[8] = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
        Elements[12] = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;
        Elements[1] = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
        Elements[5] = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
        Elements[9] = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
        Elements[13] = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;
        Elements[2] = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
        Elements[6] = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
        Elements[10] = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
        Elements[14] = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;
        Elements[3] = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
        Elements[7] = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
        Elements[11] = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
        Elements[15] = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 MultiplyScalar(float scalar)
    {
        Elements[0] *= scalar;
        Elements[4] *= scalar;
        Elements[8] *= scalar;
        Elements[12] *= scalar;
        Elements[1] *= scalar;
        Elements[5] *= scalar;
        Elements[9] *= scalar;
        Elements[13] *= scalar;
        Elements[2] *= scalar;
        Elements[6] *= scalar;
        Elements[10] *= scalar;
        Elements[14] *= scalar;
        Elements[3] *= scalar;
        Elements[7] *= scalar;
        Elements[11] *= scalar;
        Elements[15] *= scalar;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Premultiply(Matrix4 mat4)
    {
        var a11 = mat4.Elements[0];
        var a12 = mat4.Elements[4];
        var a13 = mat4.Elements[8];
        var a14 = mat4.Elements[12];
        var a21 = mat4.Elements[1];
        var a22 = mat4.Elements[5];
        var a23 = mat4.Elements[9];
        var a24 = mat4.Elements[13];
        var a31 = mat4.Elements[2];
        var a32 = mat4.Elements[6];
        var a33 = mat4.Elements[10];
        var a34 = mat4.Elements[14];
        var a41 = mat4.Elements[3];
        var a42 = mat4.Elements[7];
        var a43 = mat4.Elements[11];
        var a44 = mat4.Elements[15];
        var b11 = Elements[0];
        var b12 = Elements[4];
        var b13 = Elements[8];
        var b14 = Elements[12];
        var b21 = Elements[1];
        var b22 = Elements[5];
        var b23 = Elements[9];
        var b24 = Elements[13];
        var b31 = Elements[2];
        var b32 = Elements[6];
        var b33 = Elements[10];
        var b34 = Elements[14];
        var b41 = Elements[3];
        var b42 = Elements[7];
        var b43 = Elements[11];
        var b44 = Elements[15];

        Elements[0] = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
        Elements[4] = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
        Elements[8] = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
        Elements[12] = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;
        Elements[1] = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
        Elements[5] = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
        Elements[9] = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
        Elements[13] = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;
        Elements[2] = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
        Elements[6] = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
        Elements[10] = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
        Elements[14] = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;
        Elements[3] = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
        Elements[7] = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
        Elements[11] = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
        Elements[15] = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 RotateX(float theta) => Premultiply(new Matrix4().MakeRotationX(theta));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 RotateY(float theta) => Premultiply(new Matrix4().MakeRotationY(theta));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 RotateZ(float theta) => Premultiply(new Matrix4().MakeRotationZ(theta));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Scale(Vector3 vec3) => Premultiply(new Matrix4().MakeScale(vec3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Shear(Vector3 vec3) => Premultiply(new Matrix4().MakeShear(vec3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Translate(Vector3 vec3) => Premultiply(new Matrix4().MakeTranslation(vec3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 SetFromMatrix3(Matrix3 mat3) =>
        Set(
            mat3.Elements[0], mat3.Elements[3], mat3.Elements[6], 0,
            mat3.Elements[1], mat3.Elements[4], mat3.Elements[7], 0,
            mat3.Elements[2], mat3.Elements[5], mat3.Elements[8], 0,
            0, 0, 0, 1
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 ApplyPosition(Vector3 vec3)
    {
        Elements[12] = vec3.X;
        Elements[13] = vec3.Y;
        Elements[14] = vec3.Z;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float[] ToArray() => (float[])Elements.Clone();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4 Transpose()
    {
        var tmp = Elements[1];
        Elements[1] = Elements[4];
        Elements[4] = tmp;
        tmp = Elements[2];
        Elements[2] = Elements[8];
        Elements[8] = tmp;
        tmp = Elements[6];
        Elements[6] = Elements[9];
        Elements[9] = tmp;
        tmp = Elements[3];
        Elements[3] = Elements[12];
        Elements[12] = tmp;
        tmp = Elements[7];
        Elements[7] = Elements[13];
        Elements[13] = tmp;
        tmp = Elements[11];
        Elements[11] = Elements[14];
        Elements[14] = tmp;

        return this;
    }

    public bool Equals(Matrix4 other) => Elements.SequenceEqual(other.Elements);

    public override bool Equals(object obj) => obj is Matrix4 other && Equals(other);

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format) => ToString(format, CultureInfo.CurrentCulture);

    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format, IFormatProvider formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return
            $"""
             mat4<
                {Elements[0].ToString(format, formatProvider)}{separator} {Elements[1].ToString(format, formatProvider)}{separator} {Elements[2].ToString(format, formatProvider)}{separator} {Elements[3].ToString(format, formatProvider)}
                {Elements[4].ToString(format, formatProvider)}{separator} {Elements[5].ToString(format, formatProvider)}{separator} {Elements[6].ToString(format, formatProvider)}{separator} {Elements[7].ToString(format, formatProvider)}
                {Elements[8].ToString(format, formatProvider)}{separator} {Elements[9].ToString(format, formatProvider)}{separator} {Elements[10].ToString(format, formatProvider)}{separator} {Elements[11].ToString(format, formatProvider)}
                {Elements[12].ToString(format, formatProvider)}{separator} {Elements[13].ToString(format, formatProvider)}{separator} {Elements[14].ToString(format, formatProvider)}{separator} {Elements[15].ToString(format, formatProvider)}
             >
             """;
    }

    public override int GetHashCode() =>
        Elements is { Length: 16 }
            ? HashCode.Combine(
                HashCode.Combine(Elements[0], Elements[1], Elements[2], Elements[3]),
                HashCode.Combine(Elements[4], Elements[5], Elements[6], Elements[7]),
                HashCode.Combine(Elements[8], Elements[9], Elements[10], Elements[11]),
                HashCode.Combine(Elements[12], Elements[13], Elements[14], Elements[15])
            )
            : 0;
}