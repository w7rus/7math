using System.Numerics;
using System.Runtime.CompilerServices;

namespace _7math;

public static class Helpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max) => MathF.Max(min, MathF.Min(max, value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float start, float end, float alpha) => (1 - alpha) * start + alpha * end;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MapLinear<T>(T nValue, T nInFrom, T nInTo, T nOutFrom, T nOutTo) where T : ISignedNumber<T> =>
        nOutFrom + (nValue - nInFrom) * (nOutTo - nOutFrom) / (nInTo - nInFrom);
}